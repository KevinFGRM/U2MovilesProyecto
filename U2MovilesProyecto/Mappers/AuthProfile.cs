using AutoMapper;
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
            CreateMap<Usuarios, UsuarioResponseDTO>();
            CreateMap<Mensajes, MensajeResponseDTO>()
                .ForMember(
                    dest => dest.Contenido,
                    opt => opt.MapFrom(src => src.Mensaje))
                .ForMember(
                    dest => dest.FechaEnvio,
                    opt => opt.MapFrom(src => src.Fecha));


        }
    }
}
