namespace MiniJokeRPGAPP.Models.DTOs
{

    public class AgregarAmigoDTO
    {
        public string NombreUsuario { get; set; } = null!;
    }
    public class AceptarAmigoDTO
    {
        public int IdUsuario { get; set; }
    }

    public class AmigoResponseDTO
    {
        public int IdUsuario { get; set; }
        public string? Estado { get; set; }
        public string NombreUsuario { get; set; } = null!;
    }
    public class UsuarioResponseDTO
    {
        public int IdUsuario { get; set; }
        public string NombreUsuario { get; set; } = null!;
        public string EstadoAmistad { get; set; } = null!;
        public bool SoyEmisor { get; set; }

        public bool EsAmigo => EstadoAmistad == "aceptado";
        public bool EsPendiente => EstadoAmistad == "pendiente";
        public bool EsNinguno => EstadoAmistad == "ninguno";

    }
}
