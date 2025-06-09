using JuegoBatallasMagicas.API.Data;
using JuegoBatallasMagicas.API.DTOs;
using JuegoBatallasMagicas.API.Models;
using Microsoft.AspNetCore.Http.HttpResults; // Para resultados específicos de Minimal API
using Microsoft.EntityFrameworkCore;

namespace JuegoBatallasMagicas.API.Extensions
{
    public static class PersonajeEndpointsExtension
    {
        public static void MapPersonajeEndpoints(this IEndpointRouteBuilder app)
        {
            var personajesGroup = app.MapGroup("/api/personajes").WithTags("Personajes");

            // Endpoint para crear un personaje
            personajesGroup.MapPost("/", async (CrearPersonajeRequest request, ApplicationDbContext context) =>
            {
                // Validar si el nombre ya existe
                if (await context.Personajes.AnyAsync(p => p.Nombre == request.Nombre))
                {
                    return Results.Conflict($"Ya existe un personaje con el nombre '{request.Nombre}'.");
                }

                var nuevoPersonaje = new Personaje
                {
                    Nombre = request.Nombre,
                    Tipo = request.Tipo,
                    VidaMaxima = request.VidaMaxima,
                    ManaMaximo = request.ManaMaximo,
                    Fuerza = request.Fuerza,
                    Inteligencia = request.Inteligencia,
                    // VidaActual y ManaActual se inicializan en el constructor de Personaje
                };

                // Asegurar que la vida y el maná actuales se establecen en sus máximos
                nuevoPersonaje.VidaActual = nuevoPersonaje.VidaMaxima;
                nuevoPersonaje.ManaActual = nuevoPersonaje.ManaMaximo;

                context.Personajes.Add(nuevoPersonaje);
                await context.SaveChangesAsync();

                return Results.Created($"/api/personajes/{nuevoPersonaje.Id}", nuevoPersonaje);
            })
            .WithName("CrearPersonaje")
            .WithOpenApi();

            // Endpoint para asignar una habilidad a un personaje
            personajesGroup.MapPost("/{personajeId}/habilidades/{habilidadId}", async (int personajeId, int habilidadId, ApplicationDbContext context) =>
            {
                var personaje = await context.Personajes.FindAsync(personajeId);
                if (personaje == null)
                {
                    return Results.NotFound($"Personaje con ID {personajeId} no encontrado.");
                }

                var habilidad = await context.Habilidades.FindAsync(habilidadId);
                if (habilidad == null)
                {
                    return Results.NotFound($"Habilidad con ID {habilidadId} no encontrada.");
                }

                // Verificar si el personaje ya tiene la habilidad
                var personajeHabilidadExistente = await context.PersonajeHabilidades
                    .AnyAsync(ph => ph.PersonajeId == personajeId && ph.HabilidadId == habilidadId);

                if (personajeHabilidadExistente)
                {
                    return Results.Conflict($"El personaje '{personaje.Nombre}' ya tiene la habilidad '{habilidad.Nombre}'.");
                }

                var personajeHabilidad = new PersonajeHabilidad
                {
                    PersonajeId = personajeId,
                    HabilidadId = habilidadId
                };

                context.PersonajeHabilidades.Add(personajeHabilidad);
                await context.SaveChangesAsync();

                return Results.Ok($"Habilidad '{habilidad.Nombre}' asignada al personaje '{personaje.Nombre}' con éxito.");
            })
            .WithName("AsignarHabilidadAPersonaje")
            .WithOpenApi();

            // Endpoint para obtener un personaje por ID (útil para verificar)
            personajesGroup.MapGet("/{id}", async (int id, ApplicationDbContext context) =>
            {
                var personaje = await context.Personajes
                                    .Include(p => p.PersonajeHabilidades) // Incluir la tabla intermedia
                                        .ThenInclude(ph => ph.Habilidad) // Luego la habilidad asociada
                                    .AsNoTracking() // No rastrear cambios para consultas de solo lectura
                                    .Where(p => p.Id == id) // Usar Where para luego poder usar Select
                                    .Select(p => new PersonajeDTO // PROYECCIÓN A DTO
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
                                                        .Select(ph => new HabilidadDTO // PROYECCIÓN A HabilidadDTO
                                                        {
                                                            Id = ph.Habilidad.Id,
                                                            Nombre = ph.Habilidad.Nombre,
                                                            CosteMana = ph.Habilidad.CosteMana,
                                                            TipoEfecto = ph.Habilidad.TipoEfecto,
                                                            ValorEfecto = ph.Habilidad.ValorEfecto
                                                        })
                                                        .ToList()
                                    })
                                    .FirstOrDefaultAsync();

                return personaje is PersonajeDTO ? Results.Ok(personaje) : Results.NotFound();
            })
            .WithName("ObtenerPersonajePorId")
            .WithOpenApi();

            // Endpoint para obtener todos los personajes
            personajesGroup.MapGet("/", async (ApplicationDbContext context) =>
            {
                var personajes = await context.Personajes
                                        .Include(p => p.PersonajeHabilidades)
                                            .ThenInclude(ph => ph.Habilidad)
                                        .AsNoTracking()
                                        .Select(p => new PersonajeDTO // PROYECCIÓN A DTO
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
                                                            .Select(ph => new HabilidadDTO // PROYECCIÓN A HabilidadDTO
                                                            {
                                                                Id = ph.Habilidad.Id,
                                                                Nombre = ph.Habilidad.Nombre,
                                                                CosteMana = ph.Habilidad.CosteMana,
                                                                TipoEfecto = ph.Habilidad.TipoEfecto,
                                                                ValorEfecto = ph.Habilidad.ValorEfecto
                                                            })
                                                            .ToList()
                                        })
                                        .ToListAsync();
                return Results.Ok(personajes);
            })
            .WithName("ObtenerTodosLosPersonajes")
            .WithOpenApi();
        }
    }
}