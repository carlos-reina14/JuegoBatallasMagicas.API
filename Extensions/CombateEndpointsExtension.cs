using JuegoBatallasMagicas.API.Data;
using JuegoBatallasMagicas.API.DTOs;
using JuegoBatallasMagicas.API.Models;
using JuegoBatallasMagicas.API.Services; // ¡Importa tu servicio!
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;

namespace JuegoBatallasMagicas.API.Extensions
{
    public static class CombateEndpointsExtension
    {
        public static void MapCombateEndpoints(this IEndpointRouteBuilder app)
        {
            var combateGroup = app.MapGroup("/api/combate").WithTags("Combate");

            // Endpoint para iniciar un combate
            combateGroup.MapPost("/simular", async (SimularCombateRequest request, CombateService combateService) =>
            {
                if (request.Personaje1Id == request.Personaje2Id)
                {
                    return Results.BadRequest("Un personaje no puede luchar contra sí mismo.");
                }

                var resultado = await combateService.SimularCombate(request.Personaje1Id, request.Personaje2Id);

                // Comprobar si hubo un error de personajes no encontrados
                if (resultado.Mensaje.Contains("no encontrados"))
                {
                    return Results.NotFound(resultado);
                }
                
                return Results.Ok(resultado);
            })
            .WithName("SimularCombate")
            .WithOpenApi();

            // Nuevo Endpoint: Obtener historial de todos los combates
            combateGroup.MapGet("/historial", async (ApplicationDbContext context) =>
            {
                var combates = await context.Combates
                    .Include(c => c.Personaje1) // Incluir personajes para sus nombres
                    .Include(c => c.Personaje2)
                    .Include(c => c.Ganador) // Incluir el ganador para su nombre
                    .OrderByDescending(c => c.FechaCombate) // Ordenar por fecha para ver los más recientes
                    .Select(c => new CombateResumenDTO
                    {
                        Id = c.Id,
                        Personaje1Id = c.Personaje1Id,
                        Personaje1Nombre = c.Personaje1.Nombre,
                        Personaje2Id = c.Personaje2Id,
                        Personaje2Nombre = c.Personaje2.Nombre,
                        GanadorId = c.GanadorId,
                        GanadorNombre = c.Ganador != null ? c.Ganador.Nombre : "Empate/Límite de Turnos",
                        FechaCombate = c.FechaCombate,
                        ResultadoMensaje = c.Ganador != null ? $"{c.Ganador.Nombre} ganó." : "Empate o límite de turnos."
                    })
                    .ToListAsync();

                return Results.Ok(combates);
            })
            .WithName("ObtenerHistorialCombates")
            .WithOpenApi();

            // Nuevo Endpoint: Obtener detalles de un combate específico (incluyendo acciones)
            combateGroup.MapGet("/{id}", async (int id, ApplicationDbContext context) =>
            {
                var combate = await context.Combates
                    .Include(c => c.Personaje1)
                    .Include(c => c.Personaje2)
                    .Include(c => c.Ganador)
                    .Include(c => c.Acciones) // ¡Incluir las acciones del combate!
                        .ThenInclude(ac => ac.PersonajeAtacante) // Incluir el atacante para el nombre
                    .Include(c => c.Acciones)
                        .ThenInclude(ac => ac.PersonajeObjetivo) // Incluir el objetivo para el nombre
                    .Include(c => c.Acciones)
                        .ThenInclude(ac => ac.HabilidadUsada) // Incluir la habilidad usada para el nombre
                    .Where(c => c.Id == id)
                    .Select(c => new ResultadoCombateDTO // Reutilizamos el DTO de resultado completo
                    {
                        CombateId = c.Id,
                        Mensaje = c.Ganador != null ? $"{c.Ganador.Nombre} ganó el combate." : "Combate terminado por empate o límite de turnos.",
                        Ganador = c.Ganador != null ? new PersonajeDTO
                        {
                            Id = c.Ganador.Id,
                            Nombre = c.Ganador.Nombre,
                            Tipo = c.Ganador.Tipo,
                            VidaMaxima = c.Ganador.VidaMaxima,
                            ManaMaximo = c.Ganador.ManaMaximo,
                            Fuerza = c.Ganador.Fuerza,
                            Inteligencia = c.Ganador.Inteligencia,
                            VidaActual = c.Ganador.VidaActual, // Nota: Estos serán los valores del momento de la victoria
                            ManaActual = c.Ganador.ManaActual
                            // Habilidades no se cargan aquí para el ganador, ya que sería el estado final del combate.
                        } : null,
                        // Aquí no necesitamos el estado final de P1 y P2 directamente,
                        // ya que está en el log de acciones, pero mantenemos los campos del DTO.
                        // Podemos omitirlos o ponerlos a null si no se necesitan.
                        Personaje1EstadoFinal = null, // Se puede obtener del log si es necesario
                        Personaje2EstadoFinal = null, // Se puede obtener del log si es necesario
                        HistorialAcciones = c.Acciones
                            .OrderBy(ac => ac.NumeroTurno) // Ordenar las acciones por turno
                            .Select(ac => new AccionCombateDTO
                            {
                                NumeroTurno = ac.NumeroTurno,
                                PersonajeAtacanteId = ac.PersonajeAtacanteId,
                                PersonajeAtacanteNombre = ac.PersonajeAtacante.Nombre,
                                HabilidadUsadaId = ac.HabilidadUsadaId,
                                HabilidadUsadaNombre = ac.HabilidadUsada != null ? ac.HabilidadUsada.Nombre : "Ataque Básico",
                                PersonajeObjetivoId = ac.PersonajeObjetivoId,
                                PersonajeObjetivoNombre = ac.PersonajeObjetivo.Nombre,
                                DanoInfligido = ac.DanoInfligido,
                                CuracionAplicada = ac.CuracionAplicada,
                                EscudoAplicado = ac.EscudoAplicado,
                                VidaRestanteAtacante = ac.VidaRestanteAtacante,
                                ManaRestanteAtacante = ac.ManaRestanteAtacante,
                                VidaRestanteObjetivo = ac.VidaRestanteObjetivo,
                                ManaRestanteObjetivo = ac.ManaRestanteObjetivo
                            })
                            .ToList()
                    })
                    .FirstOrDefaultAsync();

                return combate is ResultadoCombateDTO ? Results.Ok(combate) : Results.NotFound();
            })
            .WithName("ObtenerDetallesCombate")
            .WithOpenApi();

            // Nuevo Endpoint: Obtener el conteo de victorias por personaje
            combateGroup.MapGet("/victorias", async (ApplicationDbContext context) =>
            {
                var victorias = await context.Combates
                    .Where(c => c.GanadorId.HasValue) // Solo contamos combates con un ganador definido
                    .GroupBy(c => c.GanadorId) // Agrupamos por el ID del ganador
                    .Select(g => new
                    {
                        PersonajeId = g.Key, // El ID del ganador es la clave del grupo
                        ConteoVictorias = g.Count() // Contamos cuántos combates ganó ese ID
                    })
                    .Join(context.Personajes, // Hacemos un JOIN con la tabla de Personajes para obtener el nombre
                          victoria => victoria.PersonajeId,
                          personaje => personaje.Id,
                          (victoria, personaje) => new RankingEntryDTO // Proyectamos a un nuevo DTO
                          {
                              PersonajeId = victoria.PersonajeId ?? 0, // Asegurarse de que no sea null
                              PersonajeNombre = personaje.Nombre,
                              Victorias = victoria.ConteoVictorias
                          })
                    .OrderByDescending(r => r.Victorias) // Ordenamos de forma descendente por victorias
                    .ToListAsync();

                return Results.Ok(victorias);
            })
            .WithName("ObtenerVictoriasPorPersonaje")
            .WithOpenApi();

            // Endpoint: Obtener victorias de un personaje específico (usando RankingEntryDTO)
            combateGroup.MapGet("/personaje/{id}/victorias", async (int id, ApplicationDbContext context) =>
            {
                // Primero, verifica si el personaje existe
                var personaje = await context.Personajes.AsNoTracking().FirstOrDefaultAsync(p => p.Id == id);

                if (personaje == null)
                {
                    return Results.NotFound($"Personaje con ID {id} no encontrado.");
                }

                // Cuenta las victorias de ese personaje
                var victorias = await context.Combates
                    .Where(c => c.GanadorId == id) // Filtra por el ID del personaje ganador
                    .CountAsync(); // Cuenta el número de victorias

                // Prepara la respuesta DTO, REUTILIZANDO RankingEntryDTO
                var resultado = new RankingEntryDTO
                {
                    PersonajeId = personaje.Id,
                    PersonajeNombre = personaje.Nombre,
                    Victorias = victorias
                };

                return Results.Ok(resultado);
            })
            .WithName("ObtenerVictoriasDePersonajeEnCombate")
            .WithOpenApi();
        }
    }
}