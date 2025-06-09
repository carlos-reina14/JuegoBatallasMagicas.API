using System;
using System.Collections.Generic;

namespace JuegoBatallasMagicas.API.Models
{
    public class Combate
    {
        public int Id { get; set; }
        public int Personaje1Id { get; set; }
        public Personaje Personaje1 { get; set; } = null!; // Propiedad de navegación

        public int Personaje2Id { get; set; }
        public Personaje Personaje2 { get; set; } = null!; // Propiedad de navegación

        public int? GanadorId { get; set; } // Puede ser nulo si el combate aún no ha terminado
        public Personaje? Ganador { get; set; } // Propiedad de navegación

        public DateTime FechaCombate { get; set; } = DateTime.UtcNow; // Registrar cuándo ocurrió el combate

        // Propiedad de navegación para las acciones/turnos del combate
        public ICollection<AccionCombate> Acciones { get; set; } = new List<AccionCombate>();
    }
}