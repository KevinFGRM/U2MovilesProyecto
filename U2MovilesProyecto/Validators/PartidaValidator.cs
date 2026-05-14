using FluentValidation;
using U2MovilesProyecto.Models.DTOs;

namespace U2MovilesProyecto.Validators
{
    public class CrearPartidaValidator : AbstractValidator<CrearPartidaDto>
    {
        public CrearPartidaValidator()
        {
            RuleFor(x => x.IdJugador2)
                .GreaterThan(0);
        }
    }

    public class SeleccionarPersonajeValidator : AbstractValidator<SeleccionarPersonajeDto>
    {
        public SeleccionarPersonajeValidator()
        {
            RuleFor(x => x.IdPersonaje)
                .GreaterThan(0);
        }
    }

    public class RealizarAccionValidator : AbstractValidator<RealizarAccionDto>
    {
        public RealizarAccionValidator()
        {
            RuleFor(x => x.IdHabilidad)
                .GreaterThan(0);
        }
    }
}
