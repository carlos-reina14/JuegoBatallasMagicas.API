namespace JuegoBatallasMagicas.API.Models
{
    public class AccionCombate
    {
        public int Id { get; set; }
        public int CombateId { get; set; }
        public Combate Combate { get; set; } = null!; // Propiedad de navegación

        public int NumeroTurno { get; set; }
        public int PersonajeAtacanteId { get; set; }
        public Personaje PersonajeAtacante { get; set; } = null!; // Propiedad de navegación

        public int? HabilidadUsadaId { get; set; }
        public Habilidad? HabilidadUsada { get; set; } = null!; // Propiedad de navegación

        public int PersonajeObjetivoId { get; set; }
        public Personaje PersonajeObjetivo { get; set; } = null!; // Propiedad de navegación

        public int DanoInfligido { get; set; } // Si aplica
        public int CuracionAplicada { get; set; } // Si aplica
        public int EscudoAplicado { get; set; } // Si aplica

        public int VidaRestanteAtacante { get; set; }
        public int ManaRestanteAtacante { get; set; }
        public int VidaRestanteObjetivo { get; set; }
        public int ManaRestanteObjetivo { get; set; } // En caso de que el objetivo sea curador y use maná
    }
}