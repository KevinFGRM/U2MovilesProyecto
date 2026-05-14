namespace MiniJokeRPGAPP.Models.DTOs
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
        public int Jugador1 { get; set; } // usuario
        public int Jugador2 { get; set; } // usuario
        public int TurnoActual { get; set; }
        public string Estado { get; set; } = null!;
        public DateTime? FechaInicio { get; set; }
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
}
