using System.Collections.Generic;

namespace JuegoBatallasMagicas.API.Models
{
    public class Personaje
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public TipoPersonaje Tipo { get; set; } // Mago, Guerrero, Curandero

        // Atributos base
        public int VidaMaxima { get; set; }
        public int ManaMaximo { get; set; }
        public int Fuerza { get; set; }
        public int Inteligencia { get; set; }

        // Atributos actuales (para el combate)
        public int VidaActual { get; set; }
        public int ManaActual { get; set; }

        // Propiedad de navegación para las habilidades del personaje (relación muchos a muchos)
        public ICollection<PersonajeHabilidad> PersonajeHabilidades { get; set; } = new List<PersonajeHabilidad>();

        // Constructor para inicializar vida y maná actuales
        public Personaje()
        {
            VidaActual = VidaMaxima; // Se inicializa con la vida máxima al crear
            ManaActual = ManaMaximo; // Se inicializa con el maná máximo al crear
        }
    }

    public enum TipoPersonaje
    {
        Mago,
        Guerrero,
        Curandero
    }
}