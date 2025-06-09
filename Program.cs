using Microsoft.EntityFrameworkCore;
using JuegoBatallasMagicas.API.Data;
using JuegoBatallasMagicas.API.Extensions;
using JuegoBatallasMagicas.API.Services;

var builder = WebApplication.CreateBuilder(args);

// ... (Configuración de Swagger/OpenAPI) ...
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Añadir el servicio de combate al contenedor de dependencias
builder.Services.AddScoped<CombateService>();

// ... (Configuración de Entity Framework Core) ...
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    // Usar los middlewares de Swagger UI (ahora disponibles)
    app.UseSwagger();   // Sirve el documento JSON de la especificación OpenAPI
    app.UseSwaggerUI(); // Sirve la interfaz de usuario web que consume el JSON
}

app.UseHttpsRedirection();

app.MapGet("/", () => "¡La API de Batallas Mágicas está funcionando!");

// Mapear los endpoints de Personajes y Habilidades
app.MapPersonajeEndpoints(); // Llama al método de extensión
app.MapHabilidadEndpoints(); // Llama al método de extensión
app.MapCombateEndpoints(); // ¡Mapea los endpoints de combate!

app.Run();