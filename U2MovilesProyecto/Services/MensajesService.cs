using AutoMapper;
using AvisosAPI.Repositories;
using Microsoft.EntityFrameworkCore;
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

        public ListaMensajesDTO ObtenerChat(int idUsuario2)
        {
            int idUsuario1 = ObtenerIdUsuario();

            var mensajes = mensajesRepository.Query().Include(x=>x.EmisorNavigation).Include(x=>x.ReceptorNavigation)
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
                
                return dto;
            }).ToList();

            var response = new ListaMensajesDTO
            {
                NombreAmigo = amigo ?? "",
                Mensajes = listaMensajes
            };

            return response;

            //return mensajes.Select(m => mapper.Map<MensajeResponseDTO>(m)).ToList();
        }
    }
}
