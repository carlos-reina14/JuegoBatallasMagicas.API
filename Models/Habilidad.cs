using System.Collections.Generic;

namespace JuegoBatallasMagicas.API.Models
{
    public class Habilidad
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public int CosteMana { get; set; }
        public TipoEfectoHabilidad TipoEfecto { get; set; } // Daño, Curacion, Escudo
        public int ValorEfecto { get; set; } // Cantidad de daño, curación o escudo

        // Propiedad de navegación para los personajes que tienen esta habilidad (relación muchos a muchos)
        public ICollection<PersonajeHabilidad> PersonajeHabilidades { get; set; } = new List<PersonajeHabilidad>();
    }

    public enum TipoEfectoHabilidad
    {
        Dano,
        Curacion,
        Escudo
    }
}