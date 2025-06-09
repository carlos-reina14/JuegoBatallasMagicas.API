using System.Collections.Generic;

namespace JuegoBatallasMagicas.API.DTOs
{
    public class ResultadoCombateDTO
    {
        public string Mensaje { get; set; } = string.Empty;
        public PersonajeDTO? Ganador { get; set; } // Puede ser nulo en caso de empate o límite de turnos
        public PersonajeDTO? Personaje1EstadoFinal { get; set; }
        public PersonajeDTO? Personaje2EstadoFinal { get; set; }
        public List<AccionCombateDTO> HistorialAcciones { get; set; } = new List<AccionCombateDTO>();
        public int? CombateId { get; set; } // ID del combate guardado en la BD
    }
}