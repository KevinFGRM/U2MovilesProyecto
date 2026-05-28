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

            // por cualquier cosa ya que esto no seria posible en la interfaz
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
                Estado = "pendiente",
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
            if (relacion.Estado != "pendiente")
                throw new Exception("La relación de amistad no está pendiente.");
            relacion.Estado = "aceptado";
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
            if (relacion.Estado != "pendiente")
                throw new Exception("La relación de amistad no está pendiente.");
            relacion.Estado = "rechazado";
            amigosRepository.Update(relacion);
        }

        public List<AmigoResponseDTO> ObtenerAmigos()
        {
            int idUsuario = ObtenerIdUsuario();

            var amigos = amigosRepository.Query()
                .Where(x => (x.Usuario1 == idUsuario || x.Usuario2 == idUsuario) && x.Estado == "aceptado")
                .ToList();

            List<Usuarios> usuarios = new();

            foreach (var amigo in amigos)
            {
                int idAmigo = amigo.Usuario1 == idUsuario ? amigo.Usuario2 : amigo.Usuario1;

                var usuario = usuariosRepository.Get(idAmigo);

                if (usuario != null)
                    usuarios.Add(usuario);
            }

            return usuarios.Select(u => mapper.Map<AmigoResponseDTO>(u)).ToList();
        }
        public List<UsuarioResponseDTO> ObtenerUsuarios()
        {
            int usuario = ObtenerIdUsuario();


            var usuarios = usuariosRepository.Query()
                .Where(x => x.IdUsuario != usuario)
                .ToList();

            List<UsuarioResponseDTO> lista = new();

            foreach (var item in usuarios)
            {
                // despues de lo que hice en partidas ya no me parece tan complicado

                // aqui me empece a arrepentir de no haber planeado propiedades en las entidades y/o dtos 
                // pero bueno se logro.
                var siEspendientePeroSoyElQueMandoSolicitud = amigosRepository.Query().Any(a => (a.Usuario2 == item.IdUsuario && a.Estado == "pendiente"
                && ((a.Usuario1 == usuario && a.Usuario2 == item.IdUsuario)
                            || (a.Usuario1 == item.IdUsuario && a.Usuario2 == usuario))));

                var relacion = amigosRepository.Query().FirstOrDefault(x => ((x.Usuario1 == usuario && x.Usuario2 == item.IdUsuario)
                            || (x.Usuario1 == item.IdUsuario && x.Usuario2 == usuario)));

                string estado = "ninguno";

                if (relacion != null)
                {
                    estado = relacion.Estado;
                }

                lista.Add(new UsuarioResponseDTO
                {
                    IdUsuario = item.IdUsuario,
                    NombreUsuario = item.NombreUsuario,
                    EstadoAmistad = estado,
                    SoyEmisor = siEspendientePeroSoyElQueMandoSolicitud

                });
            }

            return lista;
        }

        public List<UsuarioResponseDTO> ObtenerPendientes()
        {
            int usuario = ObtenerIdUsuario();

            var pendientes = amigosRepository.Query()
                .Where(x => x.Usuario2 == usuario && x.Estado == "pendiente")
                .ToList();

            List<UsuarioResponseDTO> lista = new();

            foreach (var item in pendientes)
            {
                var usuarioPendiente = usuariosRepository.Get(item.Usuario1);

                if (usuarioPendiente != null)
                {
                    lista.Add(new UsuarioResponseDTO
                    {
                        IdUsuario = usuarioPendiente.IdUsuario,
                        NombreUsuario = usuarioPendiente.NombreUsuario,
                        EstadoAmistad = "pendiente"
                    });
                }
            }

            return lista;
        }
    }
}
