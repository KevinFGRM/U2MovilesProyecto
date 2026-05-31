using AutoMapper;
using AvisosAPI.Repositories;
using MateAventuras_Corregido.Helpers;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using MiniJokeRPGAPI.Models.Entities;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using U2MovilesProyecto.Models.DTOs;

namespace U2MovilesProyecto.Services
{
    public class AuthService
    {
        private readonly Repository<Usuarios> usuariosRepository;
        private readonly Repository<Fcmtokens> fcmtokenRepository;
        private readonly IMapper mapper;
        private readonly IConfiguration configuration;
        private readonly IHttpContextAccessor httpContextAccessor;

        public AuthService(Repository<Usuarios> usuariosRepository, Repository<Fcmtokens> fcmtokenRepository, IMapper mapper, IConfiguration configuration, IHttpContextAccessor httpContextAccessor)
        {
            this.usuariosRepository = usuariosRepository;
            this.fcmtokenRepository = fcmtokenRepository;
            this.mapper = mapper;
            this.configuration = configuration;
            this.httpContextAccessor = httpContextAccessor;
        }

        public void RegistrarMaestro(RegisterDto dto)
        {
            var usuario = mapper.Map<Usuarios>(dto);
            usuario.ContrasenaHash = Sha256Helper.ComputeHash(dto.Contrasena);

            usuariosRepository.Insert(usuario);
        }

        public LoginResponseDto Login(LoginDto dto)
        {

            var usuario = usuariosRepository.Query()
                .FirstOrDefault(x => x.Correo == dto.Correo);

            if (usuario == null || (Sha256Helper.ComputeHash(dto.Contrasena) != usuario.ContrasenaHash))
            {
                throw new UnauthorizedAccessException("Credenciales incorrectas.");
            }

            var token = GenerarToken(usuario.IdUsuario, usuario.Correo, usuario.NombreUsuario);
            var response = mapper.Map<LoginResponseDto>(usuario);
            response.Token = token;
            return response;
        }

        private string GenerarToken(int id, string email, string Nombre)
        {
            var claims = new[]
            {
                new Claim("Id", id.ToString()),
                new Claim(ClaimTypes.Email, email),
                new Claim(ClaimTypes.Name, Nombre),
            };

            var key = configuration.GetValue<string>("Jwt:Key");

            var token = new JwtSecurityToken(
                issuer: configuration.GetValue<string>("Jwt:Issuer"),
                audience: configuration.GetValue<string>("Jwt:Audience"),
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(30),
                signingCredentials: new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key ?? "")), SecurityAlgorithms.HmacSha256)
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
        public void GuardarToken(string token)
        {
            int idUsuario = ObtenerIdUsuario();

            var existe = fcmtokenRepository.Query().FirstOrDefault(x => x.Token == token);

            if (existe != null)
                return;

            fcmtokenRepository.Insert(new Fcmtokens
            {
                IdUsuario = idUsuario,
                Token = token
            });

        }
        private int ObtenerIdUsuario()
        {
            var claim = httpContextAccessor.HttpContext!
                .User.FindFirst("Id");

            return int.Parse(claim!.Value);
        }
    }
}
