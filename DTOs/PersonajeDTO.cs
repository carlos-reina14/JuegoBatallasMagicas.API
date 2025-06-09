using JuegoBatallasMagicas.API.Models; // Para TipoPersonaje
using System.Collections.Generic;

namespace JuegoBatallasMagicas.API.DTOs
{
    public class PersonajeDTO
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public TipoPersonaje Tipo { get; set; }

        public int VidaMaxima { get; set; }
        public int ManaMaximo { get; set; }
        public int Fuerza { get; set; }
        public int Inteligencia { get; set; }
        public int VidaActual { get; set; }
        public int ManaActual { get; set; }

        // Aquí incluimos la lista de habilidades, pero usando HabilidadDTO para evitar el ciclo
        public List<HabilidadDTO> Habilidades { get; set; } = new List<HabilidadDTO>();
    }
}