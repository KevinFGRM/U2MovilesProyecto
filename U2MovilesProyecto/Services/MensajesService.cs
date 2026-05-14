using AutoMapper;
using AvisosAPI.Repositories;
using U2MovilesProyecto.Models.DTOs;
using U2MovilesProyecto.Models.Entities;

namespace U2MovilesProyecto.Services
{
    public class MensajesService
    {
        private readonly Repository<Mensajes> mensajesRepository;
        private readonly IHttpContextAccessor httpContextAccessor;
        private readonly IMapper mapper;

        public MensajesService(
            Repository<Mensajes> mensajesRepository,
            IHttpContextAccessor httpContextAccessor,
            IMapper mapper)
        {
            this.mensajesRepository = mensajesRepository;
            this.httpContextAccessor = httpContextAccessor;
            this.mapper = mapper;
        }

        private int ObtenerIdUsuario()
        {
            var claim = httpContextAccessor.HttpContext!
                .User.FindFirst("Id");

            return int.Parse(claim!.Value);
        }

        public void EnviarMensaje(MandarMensajeDTO dto)
        {
            int idUsuario = ObtenerIdUsuario();

            Mensajes mensaje = new()
            {
                Emisor = idUsuario,
                Receptor = dto.IdReceptor,
                Mensaje = dto.Contenido,
                Fecha = DateTime.Now
            };

            mensajesRepository.Insert(mensaje);
        }

        public List<MensajeResponseDTO> ObtenerChat(int idUsuario2)
        {
            int idUsuario1 = ObtenerIdUsuario();

            var mensajes = mensajesRepository.Query()
                .Where(x =>
                    (x.Emisor == idUsuario1 && x.Receptor == idUsuario2)
                    ||
                    (x.Emisor == idUsuario2 && x.Receptor == idUsuario1))
                .OrderBy(x => x.Fecha)
                .ToList();

            return mensajes.Select(m => mapper.Map<MensajeResponseDTO>(m)).ToList();
        }
    }
}
