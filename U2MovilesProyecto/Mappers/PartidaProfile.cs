using AutoMapper;
using U2MovilesProyecto.Models.DTOs;
using U2MovilesProyecto.Models.Entities;

namespace U2MovilesProyecto.Mappers
{
    public class PartidaProfile : Profile
    {
        public PartidaProfile()
        {
            CreateMap<Partidas, PartidaResponseDTO>();

            CreateMap<Accionespartida, AccionResponseDTO>();

            CreateMap<Personajes, PersonajeResponseDto>();

            CreateMap<Habilidades, HabilidadResponseDto>();
        }
    }
}
