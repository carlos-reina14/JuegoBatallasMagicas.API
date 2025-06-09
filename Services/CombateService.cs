using JuegoBatallasMagicas.API.Data;
using JuegoBatallasMagicas.API.DTOs; // Necesitamos los DTOs para la respuesta del combate
using JuegoBatallasMagicas.API.Models;
using Microsoft.EntityFrameworkCore;

namespace JuegoBatallasMagicas.API.Services
{
    public class CombateService
    {
        private readonly ApplicationDbContext _context;

        public CombateService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<ResultadoCombateDTO> SimularCombate(int personaje1Id, int personaje2Id)
        {
            // 1. Cargar Personajes y sus habilidades asociadas
            var personaje1 = await _context.Personajes
                                .Include(p => p.PersonajeHabilidades)
                                    .ThenInclude(ph => ph.Habilidad)
                                .AsNoTracking() // No rastrear cambios para la simulación inicial
                                .FirstOrDefaultAsync(p => p.Id == personaje1Id);

            var personaje2 = await _context.Personajes
                                .Include(p => p.PersonajeHabilidades)
                                    .ThenInclude(ph => ph.Habilidad)
                                .AsNoTracking()
                                .FirstOrDefaultAsync(p => p.Id == personaje2Id);

            if (personaje1 == null || personaje2 == null)
            {
                return new ResultadoCombateDTO { Mensaje = "Uno o ambos personajes no encontrados." };
            }

            // Clona los personajes para la simulación, así no modificamos los objetos originales cargados de la BD
            var p1Sim = ClonePersonajeParaCombate(personaje1);
            var p2Sim = ClonePersonajeParaCombate(personaje2);

            var logAcciones = new List<AccionCombateDTO>(); // Para registrar lo que ocurre en cada turno
            PersonajeDTO? ganador = null;
            string mensajeFinal = string.Empty;
            int turnoActual = 1;

            // 2. Determinar quién va primero
            Personaje pAtacante;
            Personaje pObjetivo;

            if (p1Sim.Inteligencia > p2Sim.Inteligencia)
            {
                pAtacante = p1Sim;
                pObjetivo = p2Sim;
            }
            else if (p2Sim.Inteligencia > p1Sim.Inteligencia)
            {
                pAtacante = p2Sim;
                pObjetivo = p1Sim;
            }
            else // Empate en inteligencia, P1 ataca primero
            {
                pAtacante = p1Sim;
                pObjetivo = p2Sim;
            }

            // Almacenar el escudo activo de cada personaje (valor restante)
            int escudoP1 = 0;
            int escudoP2 = 0;

            // 3. Bucle de Combate
            const int MAX_TURNOS = 100; // Límite para evitar bucles infinitos en casos complejos

            while (p1Sim.VidaActual > 0 && p2Sim.VidaActual > 0 && turnoActual <= MAX_TURNOS)
            {
                // Regeneración de Maná al inicio del turno de cada personaje
                p1Sim.ManaActual = Math.Min(p1Sim.ManaMaximo, p1Sim.ManaActual + (p1Sim.ManaMaximo / 10)); // ManaMaximo
                p2Sim.ManaActual = Math.Min(p2Sim.ManaMaximo, p2Sim.ManaActual + (p2Sim.ManaMaximo / 10)); // ManaMaximo

                // Seleccionar habilidad y ejecutar turno (Ahora síncrono)
                var (atacanteEstaVivo, objetivoEstaVivo) = EjecutarTurno( // SIN AWAIT AQUÍ
                    pAtacante,
                    pObjetivo,
                    ref escudoP1,
                    ref escudoP2,
                    logAcciones,
                    turnoActual
                );

                // Re-evaluar quién es el atacante y objetivo para el próximo turno
                // Esto es crucial si el orden de los personajes ha cambiado (por ejemplo, P1 ahora es P2Sim)
                if (pAtacante == p1Sim) // Si P1 fue el atacante, P2 será el siguiente atacante
                {
                    pAtacante = p2Sim;
                    pObjetivo = p1Sim;
                }
                else // Si P2 fue el atacante, P1 será el siguiente atacante
                {
                    pAtacante = p1Sim;
                    pObjetivo = p2Sim;
                }

                turnoActual++;
            }

            // 4. Determinar Ganador y Mensaje Final
            if (p1Sim.VidaActual <= 0 && p2Sim.VidaActual <= 0)
            {
                mensajeFinal = "¡Empate! Ambos personajes cayeron.";
                ganador = null; // O se podría considerar al último atacante como ganador técnico
            }
            else if (p1Sim.VidaActual <= 0)
            {
                ganador = MapPersonajeToDTO(personaje2); // El Personaje2 original es el ganador
                mensajeFinal = $"{personaje2.Nombre} ha ganado el combate!";
            }
            else if (p2Sim.VidaActual <= 0)
            {
                ganador = MapPersonajeToDTO(personaje1); // El Personaje1 original es el ganador
                mensajeFinal = $"{personaje1.Nombre} ha ganado el combate!";
            }
            else
            {
                mensajeFinal = "El combate terminó por límite de turnos sin un claro ganador.";
            }

            // 5. Guardar el Combate en la Base de Datos
            var nuevoCombate = new Combate
            {
                Personaje1Id = personaje1.Id,
                Personaje2Id = personaje2.Id,
                GanadorId = ganador?.Id, // ID del ganador si lo hay
                FechaCombate = DateTime.UtcNow
            };
            _context.Combates.Add(nuevoCombate);
            await _context.SaveChangesAsync(); // Guardar el combate para obtener su ID

            // 6. Guardar las Acciones de Combate
            foreach (var accionDto in logAcciones)
            {
                // Mapear DTO de acción a entidad AccionCombate
                var accionDb = new AccionCombate
                {
                    CombateId = nuevoCombate.Id, // Asignar el ID del combate recién creado
                    NumeroTurno = accionDto.NumeroTurno,
                    PersonajeAtacanteId = accionDto.PersonajeAtacanteId,
                    HabilidadUsadaId = accionDto.HabilidadUsadaId,
                    PersonajeObjetivoId = accionDto.PersonajeObjetivoId,
                    DanoInfligido = accionDto.DanoInfligido,
                    CuracionAplicada = accionDto.CuracionAplicada,
                    EscudoAplicado = accionDto.EscudoAplicado,
                    VidaRestanteAtacante = accionDto.VidaRestanteAtacante,
                    ManaRestanteAtacante = accionDto.ManaRestanteAtacante,
                    VidaRestanteObjetivo = accionDto.VidaRestanteObjetivo,
                    ManaRestanteObjetivo = accionDto.ManaRestanteObjetivo
                };
                _context.AccionesCombate.Add(accionDb);
            }
            await _context.SaveChangesAsync(); // Guardar todas las acciones

            // 7. Preparar ResultadoFinalDTO
            return new ResultadoCombateDTO
            {
                Mensaje = mensajeFinal,
                Ganador = ganador,
                Personaje1EstadoFinal = MapPersonajeToDTO(p1Sim),
                Personaje2EstadoFinal = MapPersonajeToDTO(p2Sim),
                HistorialAcciones = logAcciones,
                CombateId = nuevoCombate.Id
            };
        }

        // Método EjecutarTurno
        private (bool atacanteEstaVivo, bool objetivoEstaVivo) EjecutarTurno(
            Personaje atacante,
            Personaje objetivo,
            ref int escudoAtacante,
            ref int escudoObjetivo,
            List<AccionCombateDTO> logAcciones,
            int numeroTurno)
        {
            Habilidad? habilidadElegida = null;
            int danoInfligido = 0;
            int curacionAplicada = 0;
            int escudoAplicado = 0;
            bool esAtaqueBasico = false;

            // Obtener habilidades disponibles que el personaje puede pagar
            var habilidadesDisponibles = atacante.PersonajeHabilidades
                                              .Select(ph => ph.Habilidad)
                                              .Where(h => h.CosteMana <= atacante.ManaActual)
                                              .ToList();

            // --- Lógica de Selección de Habilidad (Estrategia Simple de IA) ---

            // Prioridad 1: Curarse si la vida está baja (ej. < 30%)
            if (atacante.VidaActual < (atacante.VidaMaxima * 0.3) && habilidadesDisponibles.Any(h => h.TipoEfecto == TipoEfectoHabilidad.Curacion))
            {
                habilidadElegida = habilidadesDisponibles
                                    .Where(h => h.TipoEfecto == TipoEfectoHabilidad.Curacion)
                                    .OrderByDescending(h => h.ValorEfecto) // Elige la curación más potente
                                    .FirstOrDefault();
            }

            // Prioridad 2: Escudo si no se curó y tiene habilidad de escudo
            if (habilidadElegida == null && habilidadesDisponibles.Any(h => h.TipoEfecto == TipoEfectoHabilidad.Escudo))
            {
                 habilidadElegida = habilidadesDisponibles
                                    .Where(h => h.TipoEfecto == TipoEfectoHabilidad.Escudo)
                                    .OrderByDescending(h => h.ValorEfecto) // Elige el escudo más potente
                                    .FirstOrDefault();
            }

            // Prioridad 3: Atacar con la habilidad de daño más potente si no se ha elegido nada
            if (habilidadElegida == null && habilidadesDisponibles.Any(h => h.TipoEfecto == TipoEfectoHabilidad.Dano))
            {
                habilidadElegida = habilidadesDisponibles
                                    .Where(h => h.TipoEfecto == TipoEfectoHabilidad.Dano)
                                    .OrderByDescending(h => h.ValorEfecto) // Elige el daño más potente
                                    .FirstOrDefault();
            }


            // Si no se pudo elegir ninguna habilidad (no hay maná, no hay habilidades adecuadas), realizar Ataque Básico
            if (habilidadElegida == null)
            {
                esAtaqueBasico = true;
                danoInfligido = Math.Max(0, atacante.Fuerza / 2); // Calcular el daño del ataque básico
            }
            else
            {
                atacante.ManaActual -= habilidadElegida.CosteMana; // Consumir maná de la habilidad
            }

            // Aplicar el efecto (ataque básico o habilidad)
            if (esAtaqueBasico)
            {
                // Aplicar el daño del ataque básico al objetivo, considerando su escudo
                if (escudoObjetivo > 0)
                {
                    int danoReal = Math.Max(0, danoInfligido - escudoObjetivo);
                    escudoObjetivo = Math.Max(0, escudoObjetivo - danoInfligido); // El escudo se consume
                    danoInfligido = danoReal; // El daño final es el que se aplica
                }
                objetivo.VidaActual -= danoInfligido;
            }
            else // Se usó una habilidad real
            {
                switch (habilidadElegida!.TipoEfecto)
                {
                    case TipoEfectoHabilidad.Dano:
                        danoInfligido = Math.Max(0, habilidadElegida.ValorEfecto + (atacante.Fuerza / 5));
                        if (escudoObjetivo > 0)
                        {
                            int danoReal = Math.Max(0, danoInfligido - escudoObjetivo);
                            escudoObjetivo = Math.Max(0, escudoObjetivo - danoInfligido);
                            danoInfligido = danoReal;
                        }
                        objetivo.VidaActual -= danoInfligido;
                        break;

                    case TipoEfectoHabilidad.Curacion:
                        curacionAplicada = Math.Max(0, habilidadElegida.ValorEfecto + (atacante.Inteligencia / 5));
                        atacante.VidaActual = Math.Min(atacante.VidaMaxima, atacante.VidaActual + curacionAplicada);
                        break;

                    case TipoEfectoHabilidad.Escudo:
                        escudoAplicado = habilidadElegida.ValorEfecto;
                        escudoAtacante = escudoAplicado;
                        break;
                }
            }

            // Registrar la acción
            logAcciones.Add(new AccionCombateDTO
            {
                NumeroTurno = numeroTurno,
                PersonajeAtacanteId = atacante.Id,
                PersonajeAtacanteNombre = atacante.Nombre,
                HabilidadUsadaId = esAtaqueBasico ? null : habilidadElegida!.Id,
                HabilidadUsadaNombre = esAtaqueBasico ? "Ataque Básico" : habilidadElegida!.Nombre,
                PersonajeObjetivoId = objetivo.Id,
                PersonajeObjetivoNombre = objetivo.Nombre,
                DanoInfligido = danoInfligido,
                CuracionAplicada = curacionAplicada,
                EscudoAplicado = escudoAplicado,
                VidaRestanteAtacante = atacante.VidaActual,
                ManaRestanteAtacante = atacante.ManaActual,
                VidaRestanteObjetivo = objetivo.VidaActual,
                ManaRestanteObjetivo = objetivo.ManaActual
            });

            return (atacante.VidaActual > 0, objetivo.VidaActual > 0);
        }

        // Método auxiliar para clonar personajes para la simulación
        // Importante: Esto NO CLONA las referencias de Habilidades dentro de PersonajeHabilidades.
        // Pero como las Habilidades no se modifican, está bien.
        // Solo necesitamos que Personaje sea una nueva instancia para modificar VidaActual/ManaActual
        private Personaje ClonePersonajeParaCombate(Personaje p)
        {
            return new Personaje
            {
                Id = p.Id,
                Nombre = p.Nombre,
                Tipo = p.Tipo,
                VidaMaxima = p.VidaMaxima,
                ManaMaximo = p.ManaMaximo,
                Fuerza = p.Fuerza,
                Inteligencia = p.Inteligencia,
                VidaActual = p.VidaMaxima, // Se reinicia al máximo para el combate
                ManaActual = p.ManaMaximo, // Se reinicia al máximo para el combate
                // Las habilidades no se clonan en profundidad, se usa la referencia a las Habilidades originales
                PersonajeHabilidades = p.PersonajeHabilidades
                                        .Select(ph => new PersonajeHabilidad { PersonajeId = ph.PersonajeId, HabilidadId = ph.HabilidadId, Habilidad = ph.Habilidad, Personaje = ph.Personaje })
                                        .ToList()
            };
        }


        // Método auxiliar para mapear Personaje a PersonajeDTO
        private PersonajeDTO MapPersonajeToDTO(Personaje p)
        {
            if (p == null) return null!;
            return new PersonajeDTO
            {
                Id = p.Id,
                Nombre = p.Nombre,
                Tipo = p.Tipo,
                VidaMaxima = p.VidaMaxima,
                ManaMaximo = p.ManaMaximo,
                Fuerza = p.Fuerza,
                Inteligencia = p.Inteligencia,
                VidaActual = p.VidaActual,
                ManaActual = p.ManaActual,
                Habilidades = p.PersonajeHabilidades
                                .Select(ph => new HabilidadDTO
                                {
                                    Id = ph.Habilidad.Id,
                                    Nombre = ph.Habilidad.Nombre,
                                    CosteMana = ph.Habilidad.CosteMana,
                                    TipoEfecto = ph.Habilidad.TipoEfecto,
                                    ValorEfecto = ph.Habilidad.ValorEfecto
                                })
                                .ToList()
            };
        }
    }
}