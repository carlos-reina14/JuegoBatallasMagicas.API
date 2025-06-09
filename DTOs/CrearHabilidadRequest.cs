using JuegoBatallasMagicas.API.Models; // Para TipoEfectoHabilidad
using System.ComponentModel.DataAnnotations;

namespace JuegoBatallasMagicas.API.DTOs
{
    public class CrearHabilidadRequest
    {
        [Required(ErrorMessage = "El nombre de la habilidad es obligatorio.")]
        [MaxLength(100, ErrorMessage = "El nombre no puede exceder los 100 caracteres.")]
        public string Nombre { get; set; } = string.Empty;

        [Range(0, 200, ErrorMessage = "El coste de maná debe estar entre 0 y 200.")]
        public int CosteMana { get; set; } = 0; // Por defecto no cuesta maná

        [Required(ErrorMessage = "El tipo de efecto de la habilidad es obligatorio.")]
        [EnumDataType(typeof(TipoEfectoHabilidad), ErrorMessage = "Tipo de efecto inválido.")]
        public TipoEfectoHabilidad TipoEfecto { get; set; }

        [Range(0, 500, ErrorMessage = "El valor del efecto debe estar entre 0 y 500.")]
        public int ValorEfecto { get; set; } = 0; // Por defecto no tiene efecto
    }
}