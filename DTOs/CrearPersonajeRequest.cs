using JuegoBatallasMagicas.API.Models; // Para TipoPersonaje
using System.ComponentModel.DataAnnotations; // Para validaciones

namespace JuegoBatallasMagicas.API.DTOs
{
    public class CrearPersonajeRequest
    {
        [Required(ErrorMessage = "El nombre del personaje es obligatorio.")]
        [MaxLength(100, ErrorMessage = "El nombre no puede exceder los 100 caracteres.")]
        public string Nombre { get; set; } = string.Empty;

        [Required(ErrorMessage = "El tipo de personaje es obligatorio.")]
        [EnumDataType(typeof(TipoPersonaje), ErrorMessage = "Tipo de personaje inválido.")]
        public TipoPersonaje Tipo { get; set; }

        [Range(1, 1000, ErrorMessage = "La Vida Máxima debe estar entre 1 y 1000.")]
        public int VidaMaxima { get; set; } = 100; // Valor por defecto
        
        [Range(1, 500, ErrorMessage = "El Maná Máximo debe estar entre 1 y 500.")]
        public int ManaMaximo { get; set; } = 50; // Valor por defecto

        [Range(1, 100, ErrorMessage = "La Fuerza debe estar entre 1 y 100.")]
        public int Fuerza { get; set; } = 10; // Valor por defecto

        [Range(1, 100, ErrorMessage = "La Inteligencia debe estar entre 1 y 100.")]
        public int Inteligencia { get; set; } = 10; // Valor por defecto
    }
}