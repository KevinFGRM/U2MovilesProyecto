namespace U2MovilesProyecto.Models.DTOs
{
    public class MandarMensajeDTO
    {
        public int IdReceptor { get; set; } // al que va dirigido el mensaje
        public string Contenido { get; set; } = null!;
    }

    public class MensajeResponseDTO
    {
        public int IdMensaje { get; set; }
        public int IdEmisor { get; set; } // el que envía el mensaje
        public int IdReceptor { get; set; } // al que va dirigido el mensaje
        public string Contenido { get; set; } = null!;
        public DateTime? FechaEnvio { get; set; }
    }
}
