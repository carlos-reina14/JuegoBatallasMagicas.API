using JuegoBatallasMagicas.API.Models; // Para TipoEfectoHabilidad

namespace JuegoBatallasMagicas.API.DTOs
{
    public class HabilidadDTO
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public int CosteMana { get; set; }
        public TipoEfectoHabilidad TipoEfecto { get; set; }
        public int ValorEfecto { get; set; }
    }
}