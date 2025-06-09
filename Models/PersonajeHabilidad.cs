namespace JuegoBatallasMagicas.API.Models
{
    public class PersonajeHabilidad
    {
        public int PersonajeId { get; set; }
        public Personaje Personaje { get; set; } = null!; // Propiedad de navegación

        public int HabilidadId { get; set; }
        public Habilidad Habilidad { get; set; } = null!; // Propiedad de navegación
    }
}