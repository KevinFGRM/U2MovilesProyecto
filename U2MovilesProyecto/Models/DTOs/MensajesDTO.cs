namespace U2MovilesProyecto.Models.DTOs
{
    public class MandarMensajeDTO
    {
        public int IdReceptor { get; set; } // al que va dirigido el mensaje
        public string Contenido { get; set; } = null!;
    }

    public class ListaMensajesDTO
    {
        public string NombreAmigo { get; set; } = null!; // para mostrar el nombre del amigo en la lista de chats
        public List<MensajeResponseDTO> Mensajes { get; set; } = null!;
    }

    public class MensajeResponseDTO
    {
        public int IdMensaje { get; set; }
        public int IdEmisor { get; set; } // el que envía el mensaje
        public int IdReceptor { get; set; } // al que va dirigido el mensaje
        public string Contenido { get; set; } = null!;
        public DateTime? FechaEnvio { get; set; }
        public bool EsEmisor { get; set; } // para saber si el mensaje lo envio el usuario actual o no
        public string Tipo { get; set; } = null!;
        public string? Archivo { get; set; }

    }
    public class EnviarImagenDTO
    {
        public int Receptor { get; set; }
        public string ImagenBase64 { get; set; } = null!;
    }
}
