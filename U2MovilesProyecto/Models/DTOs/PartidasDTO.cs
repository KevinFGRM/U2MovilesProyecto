using U2MovilesProyecto.Models.Entities;

namespace U2MovilesProyecto.Models.DTOs
{
    public class CrearPartidaDto
    {
        public int IdJugador2 { get; set; } 
    }

    public class SeleccionarPersonajeDto
    {
        public int IdPartida { get; set; }
        public int IdPersonaje { get; set; }
    }

    public class RealizarAccionDto
    {
        public int IdPartida { get; set; }
        public int IdHabilidad { get; set; }
    }

    public class PartidaResponseDTO
    {
        public int IdPartida { get; set; }
        public string? Jugador1 { get; set; } // usuario
        public string? Jugador2 { get; set; } // usuario
        public int TurnoActual { get; set; }
        public string Estado { get; set; } = null!;
        public DateTime? FechaInicio { get; set; }
        public bool JugadorActualEligio { get; set; } 
        public bool OponenteEligio { get; set; }
    }

    public class AccionResponseDTO
    {
        public int IdHistorial { get; set; }
        public int Usuario { get; set; }
        public int Habilidad { get; set; }
        public string? Descripcion { get; set; }
        public DateTime? Fecha { get; set; }
    }

    public class PersonajeResponseDto
    {
        public int IdPersonaje { get; set; }
        public string Nombre { get; set; } = null!;
        public int VidaBase { get; set; }
        public int ManaBase { get; set; }
        public int AtaqueBase { get; set; }
        public int DefensaBase { get; set; }
    }

    public class HabilidadResponseDto
    {
        public int IdHabilidad { get; set; }
        public int IdPersonaje { get; set; }
        public string Nombre { get; set; } = null!;
        public string? Descripcion { get; set; }
        public int CostoMana { get; set; }
        public int Dano { get; set; }
        public int Curacion { get; set; }
    }

    public class EstadoPartidaDto
    {
        public string? Jugador1 { get; set; } // usuario
        public string? Jugador2 { get; set; } // usuario
        public int Personaje1 { get; set; }
        public int Personaje2 { get; set; }
        public int VidaJugador1 { get; set; }
        public int VidaEnemigo2 { get; set; }
        public int ManaJugador1 { get; set; }
        public int ManaEnemigo2 { get; set; }
        public bool MiTurno { get; set; }
        public string Estado { get; set; } = "";
        public List<HabilidadResponseDto> HabilidadesJugador1 { get; set; } = new List<HabilidadResponseDto>();

    }
    public class EntrarPartidaResult
    {
        public PartidaResponseDTO? Pendiente { get; set; }
        public EstadoPartidaDto? Partida { get; set; }
    }
}
