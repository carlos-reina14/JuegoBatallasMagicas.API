using Microsoft.EntityFrameworkCore;
using JuegoBatallasMagicas.API.Data;
using JuegoBatallasMagicas.API.Extensions;
using JuegoBatallasMagicas.API.Services;

var builder = WebApplication.CreateBuilder(args);

// ... (Configuraci�n de Swagger/OpenAPI) ...
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// A�adir el servicio de combate al contenedor de dependencias
builder.Services.AddScoped<CombateService>();

// ... (Configuraci�n de Entity Framework Core) ...
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    // Usar los middlewares de Swagger UI (ahora disponibles)
    app.UseSwagger();   // Sirve el documento JSON de la especificaci�n OpenAPI
    app.UseSwaggerUI(); // Sirve la interfaz de usuario web que consume el JSON
}

app.UseHttpsRedirection();

app.MapGet("/", () => "�La API de Batallas M�gicas est� funcionando!");

// Mapear los endpoints de Personajes y Habilidades
app.MapPersonajeEndpoints(); // Llama al m�todo de extensi�n
app.MapHabilidadEndpoints(); // Llama al m�todo de extensi�n
app.MapCombateEndpoints(); // �Mapea los endpoints de combate!

app.Run();