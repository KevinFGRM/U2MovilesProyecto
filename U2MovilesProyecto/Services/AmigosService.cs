using AutoMapper;
using AvisosAPI.Repositories;
using U2MovilesProyecto.Models.DTOs;
using U2MovilesProyecto.Models.Entities;

namespace U2MovilesProyecto.Services
{
    public class AmigosService
    {
        private readonly Repository<Usuarios> usuariosRepository;
        private readonly Repository<Amigos> amigosRepository;
        private readonly IHttpContextAccessor httpContextAccessor;
        private readonly IMapper mapper;

        public AmigosService(
            Repository<Usuarios> usuariosRepository,
            Repository<Amigos> amigosRepository,
            IHttpContextAccessor httpContextAccessor,
            IMapper mapper)
        {
            this.usuariosRepository = usuariosRepository;
            this.amigosRepository = amigosRepository;
            this.httpContextAccessor = httpContextAccessor;
            this.mapper = mapper;
        }

        private int ObtenerIdUsuario()
        {
            var claim = httpContextAccessor.HttpContext!
                .User.FindFirst("Id");

            return int.Parse(claim!.Value);
        }

        public void AgregarAmigo(AgregarAmigoDTO dto)
        {
            int idUsuario = ObtenerIdUsuario();

            var amigo = usuariosRepository.Query()
                .FirstOrDefault(x => x.NombreUsuario == dto.NombreUsuario);

            if (amigo == null)
                throw new Exception("Usuario no encontrado.");

            // por cualquier cosa ya que esto no seria posible en la interfaz pero por si acaso
            if (amigo.IdUsuario == idUsuario)
                throw new Exception("No puedes agregarte a ti mismo.");

            bool existe = amigosRepository.Query().Any(x =>
                (x.Usuario1 == idUsuario && x.Usuario2 == amigo.IdUsuario) || (x.Usuario1 == amigo.IdUsuario && x.Usuario2 == idUsuario));

            if (existe)
                throw new Exception("Ya son amigos.");
            // lo que ocurre a continuacion es una peticion para ser amigos
            Amigos relacion = new()
            {
                Usuario1 = idUsuario,
                Usuario2 = amigo.IdUsuario,
                Estado = "Pendiente",
                Fecha = DateTime.Now
            };

            amigosRepository.Insert(relacion);
        }
        public void AceptarAmigo(AceptarAmigoDTO idAmigo)
        {
            int idUsuario = ObtenerIdUsuario();
            var relacion = amigosRepository.Query()
                .FirstOrDefault(x =>
                    (x.Usuario1 == idUsuario && x.Usuario2 == idAmigo.IdUsuario) ||
                    (x.Usuario1 == idAmigo.IdUsuario && x.Usuario2 == idUsuario));
            if (relacion == null)
                throw new Exception("Relación de amistad no encontrada.");
            if (relacion.Estado != "Pendiente")
                throw new Exception("La relación de amistad no está pendiente.");
            relacion.Estado = "Aceptada";
            amigosRepository.Update(relacion);
        }
        public void RechazarAmigo(AceptarAmigoDTO idAmigo)
        {
            int idUsuario = ObtenerIdUsuario();
            var relacion = amigosRepository.Query()
                .FirstOrDefault(x =>
                    (x.Usuario1 == idUsuario && x.Usuario2 == idAmigo.IdUsuario) ||
                    (x.Usuario1 == idAmigo.IdUsuario && x.Usuario2 == idUsuario));
            if (relacion == null)
                throw new Exception("Relación de amistad no encontrada.");
            if (relacion.Estado != "Pendiente")
                throw new Exception("La relación de amistad no está pendiente.");
            relacion.Estado = "Rechazada";
            amigosRepository.Update(relacion);
        }

        public List<AmigoResponseDTO> ObtenerAmigos()
        {
            int idUsuario = ObtenerIdUsuario();

            var amigos = amigosRepository.Query()
                .Where(x => x.Usuario1 == idUsuario || x.Usuario2 == idUsuario)
                .ToList();

            List<Usuarios> usuarios = new();

            foreach (var amigo in amigos)
            {
                int idAmigo = amigo.Usuario1 == idUsuario
                    ? amigo.Usuario2
                    : amigo.Usuario1;

                var usuario = usuariosRepository.Get(idAmigo);

                if (usuario != null)
                    usuarios.Add(usuario);
            }

            return usuarios.Select(u => mapper.Map<AmigoResponseDTO>(u)).ToList();
        }
    }
}
