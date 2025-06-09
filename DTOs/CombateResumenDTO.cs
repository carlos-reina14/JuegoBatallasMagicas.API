namespace JuegoBatallasMagicas.API.DTOs
{
    public class CombateResumenDTO
    {
        public int Id { get; set; }
        public int Personaje1Id { get; set; }
        public string Personaje1Nombre { get; set; } = string.Empty;
        public int Personaje2Id { get; set; }
        public string Personaje2Nombre { get; set; } = string.Empty;
        public int? GanadorId { get; set; }
        public string? GanadorNombre { get; set; }
        public DateTime FechaCombate { get; set; }
        public string ResultadoMensaje { get; set; } = string.Empty; // Breve mensaje del resultado
    }
}