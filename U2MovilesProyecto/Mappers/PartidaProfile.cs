using AutoMapper;
using MiniJokeRPGAPI.Models.Entities;
using U2MovilesProyecto.Models.DTOs;

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
