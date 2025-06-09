using System.ComponentModel.DataAnnotations;

namespace JuegoBatallasMagicas.API.DTOs
{
    public class SimularCombateRequest
    {
        [Required(ErrorMessage = "El ID del Personaje 1 es obligatorio.")]
        [Range(1, int.MaxValue, ErrorMessage = "El ID del Personaje 1 debe ser un número positivo.")]
        public int Personaje1Id { get; set; }

        [Required(ErrorMessage = "El ID del Personaje 2 es obligatorio.")]
        [Range(1, int.MaxValue, ErrorMessage = "El ID del Personaje 2 debe ser un número positivo.")]
        public int Personaje2Id { get; set; }
    }
}