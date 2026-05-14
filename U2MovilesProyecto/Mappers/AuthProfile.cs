using AutoMapper;
using MateAventuras_Corregido.Helpers;
using U2MovilesProyecto.Models.DTOs;
using U2MovilesProyecto.Models.Entities;

namespace U2MovilesProyecto.Mappers
{
    public class AuthProfile : Profile
    {
        public AuthProfile()
        {
            CreateMap<RegisterDto, Usuarios>()
                .ForMember(dest => dest.ContrasenaHash, opt => opt.MapFrom(src => src.Contrasena));

            CreateMap<Usuarios, LoginResponseDto>()
            .ForMember(dest => dest.Token, opt => opt.Ignore());

            CreateMap<Usuarios, AmigoResponseDTO>();

            CreateMap<Mensajes, MensajeResponseDTO>();

        }
    }
}
