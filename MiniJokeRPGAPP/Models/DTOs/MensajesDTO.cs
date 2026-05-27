namespace MiniJokeRPGAPP.Models.DTOs
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
        public string NombreAmigo { get; set; } = null!; // para mostrar el nombre del amigo en la lista de chats
        public string Contenido { get; set; } = null!;
        public DateTime? FechaEnvio { get; set; }
        public bool EsEmisor { get; set; } // para saber si el mensaje lo envio el usuario actual o no
        public bool NoEsEmisor => !EsEmisor;
    }
}
