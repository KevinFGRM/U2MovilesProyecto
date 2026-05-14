namespace MiniJokeRPGAPP.Models.DTOs
{
    public class LoginDto
    {
        public string Correo { get; set; } = null!;
        public string Contrasena { get; set; } = null!;
    }
    public class RegisterDto
    {
        public string NombreUsuario { get; set; } = null!;
        public string Correo { get; set; } = null!;
        public string Contrasena { get; set; } = null!;
    }
    public class LoginResponseDto
    {
        public int IdUsuario { get; set; }
        public string NombreUsuario { get; set; } = null!;
        public string Token { get; set; } = null!;
    }
}
