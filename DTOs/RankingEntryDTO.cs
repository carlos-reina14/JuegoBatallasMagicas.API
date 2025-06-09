namespace JuegoBatallasMagicas.API.DTOs
{
    public class RankingEntryDTO
    {
        public int PersonajeId { get; set; }
        public string PersonajeNombre { get; set; } = string.Empty;
        public int Victorias { get; set; }
    }
}