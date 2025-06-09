using JuegoBatallasMagicas.API.Models; // Para TipoEfectoHabilidad

namespace JuegoBatallasMagicas.API.DTOs
{
    public class AccionCombateDTO
    {
        public int NumeroTurno { get; set; }
        public int PersonajeAtacanteId { get; set; }
        public string? PersonajeAtacanteNombre { get; set; } // Nombre para el log
        public int? HabilidadUsadaId { get; set; }
        public string? HabilidadUsadaNombre { get; set; } // Nombre para el log
        public int PersonajeObjetivoId { get; set; }
        public string? PersonajeObjetivoNombre { get; set; } // Nombre para el log

        public int DanoInfligido { get; set; }
        public int CuracionAplicada { get; set; }
        public int EscudoAplicado { get; set; }

        public int VidaRestanteAtacante { get; set; }
        public int ManaRestanteAtacante { get; set; }
        public int VidaRestanteObjetivo { get; set; }
        public int ManaRestanteObjetivo { get; set; }
    }
}