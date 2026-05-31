using AutoMapper;
using AvisosAPI.Repositories;
using Microsoft.EntityFrameworkCore;
using MiniJokeRPGAPI.Models.Entities;
using U2MovilesProyecto.Models.DTOs;

namespace U2MovilesProyecto.Services
{
    public class MensajesService
    {
        private readonly Repository<Mensajes> mensajesRepository;
        private readonly Repository<Usuarios> usuariosRepository;
        private readonly IHttpContextAccessor httpContextAccessor;
        private readonly IConfiguration configuration;
        private readonly IMapper mapper;
        private readonly NotificacionesService notificacionesService;

        public MensajesService(
            Repository<Mensajes> mensajesRepository,
            Repository<Usuarios> usuariosRepository,
            IHttpContextAccessor httpContextAccessor,
            IConfiguration configuration,
            NotificacionesService notificacionesService
            IMapper mapper)
        {
            this.mensajesRepository = mensajesRepository;
            this.usuariosRepository = usuariosRepository;
            this.httpContextAccessor = httpContextAccessor;
            this.notificacionesService = notificacionesService;
            this.configuration = configuration;
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


            notificacionesService.EnviarNotificacion(
                        dto.IdReceptor,
                        "Nuevo mensaje",
                        "Te enviaron un mensaje"
                    );            //var tokens = context.Fcmtokens
            //    .Where(x => x.IdUsuario == idReceptor)
            //    .Select(x => x.Token)
            //    .ToList();

            //notificacionesService.EnviarNotificaciones(
            //    tokens,
            //    "Nuevo mensaje",
            //    "Te enviaron un mensaje"
            //);
        }

        public ListaMensajesDTO ObtenerChat(int idUsuario2)
        {
            int idUsuario1 = ObtenerIdUsuario();

            var mensajes = mensajesRepository.Query().Include(x => x.EmisorNavigation).Include(x => x.ReceptorNavigation)
                .Where(x => (x.Emisor == idUsuario1 && x.Receptor == idUsuario2) || (x.Emisor == idUsuario2 && x.Receptor == idUsuario1))
                .OrderBy(x => x.Fecha)
                .ToList();

            var amigo = mensajes.FirstOrDefault(m => m.Emisor == idUsuario2)?.EmisorNavigation?.NombreUsuario
                        ?? mensajes.FirstOrDefault(m => m.Receptor == idUsuario2)?.ReceptorNavigation?.NombreUsuario;

            // mapear a un dto individualmente para poder asignar la propiedad EsEmisor
            var listaMensajes = mensajes.Select(m =>
            {
                var dto = mapper.Map<MensajeResponseDTO>(m);
                dto.EsEmisor = m.Emisor == idUsuario1;
                if (!string.IsNullOrEmpty(dto.Archivo))
                {
                    dto.Archivo =
                        $"{configuration["BaseUrl"]}/Mensajes/{dto.Archivo}";
                }
                return dto;
            }).ToList();

            var response = new ListaMensajesDTO
            {
                NombreAmigo = amigo ?? usuariosRepository.Get(idUsuario2)?.NombreUsuario ?? "",
                Mensajes = listaMensajes
            };

            return response;

            //return mensajes.Select(m => mapper.Map<MensajeResponseDTO>(m)).ToList();
        }
        public void EnviarImagen(EnviarImagenDTO dto, string uploadsPath)
        {
            int emisor = ObtenerIdUsuario();

            var mensaje = new Mensajes()
            {
                Emisor = emisor,
                Receptor = dto.Receptor,
                Mensaje = "",
                Tipo = "imagen",
                Fecha = DateTime.Now
            };

            mensajesRepository.Insert(mensaje);

            var nombreArchivo = $"{mensaje.IdMensaje}.jpg";
            Directory.CreateDirectory(uploadsPath);
            var ruta = Path.Combine(uploadsPath, nombreArchivo);
            var bytes = Convert.FromBase64String(dto.ImagenBase64);
            File.WriteAllBytes(ruta, bytes);
            mensaje.Archivo = nombreArchivo;
            
            mensajesRepository.Update(mensaje);
        }
    }
}
