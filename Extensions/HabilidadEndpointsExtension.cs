using JuegoBatallasMagicas.API.Data;
using JuegoBatallasMagicas.API.DTOs;
using JuegoBatallasMagicas.API.Models;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;

namespace JuegoBatallasMagicas.API.Extensions
{
    public static class HabilidadEndpointsExtension
    {
        public static void MapHabilidadEndpoints(this IEndpointRouteBuilder app)
        {
            var habilidadesGroup = app.MapGroup("/api/habilidades").WithTags("Habilidades");

            // Endpoint para crear una habilidad
            habilidadesGroup.MapPost("/", async (CrearHabilidadRequest request, ApplicationDbContext context) =>
            {
                // Validar si la habilidad ya existe
                if (await context.Habilidades.AnyAsync(h => h.Nombre == request.Nombre))
                {
                    return Results.Conflict($"Ya existe una habilidad con el nombre '{request.Nombre}'.");
                }

                var nuevaHabilidad = new Habilidad
                {
                    Nombre = request.Nombre,
                    CosteMana = request.CosteMana,
                    TipoEfecto = request.TipoEfecto,
                    ValorEfecto = request.ValorEfecto
                };

                context.Habilidades.Add(nuevaHabilidad);
                await context.SaveChangesAsync();

                return Results.Created($"/api/habilidades/{nuevaHabilidad.Id}", nuevaHabilidad);
            })
            .WithName("CrearHabilidad")
            .WithOpenApi();

            // Endpoint para obtener una habilidad por ID
            habilidadesGroup.MapGet("/{id}", async (int id, ApplicationDbContext context) =>
            {
                var habilidad = await context.Habilidades.FindAsync(id);
                return habilidad is Habilidad ? Results.Ok(habilidad) : Results.NotFound();
            })
            .WithName("ObtenerHabilidadPorId")
            .WithOpenApi();

            // Endpoint para obtener todas las habilidades
            habilidadesGroup.MapGet("/", async (ApplicationDbContext context) =>
            {
                var habilidades = await context.Habilidades.AsNoTracking().ToListAsync();
                return Results.Ok(habilidades);
            })
            .WithName("ObtenerTodasLasHabilidades")
            .WithOpenApi();
        }
    }
}