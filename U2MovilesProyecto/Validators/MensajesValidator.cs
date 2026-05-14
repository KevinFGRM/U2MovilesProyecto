using FluentValidation;
using U2MovilesProyecto.Models.DTOs;

namespace U2MovilesProyecto.Validators
{
    public class MensajesValidator : AbstractValidator<MandarMensajeDTO>
    {
        public MensajesValidator()
        {
            RuleFor(x => x.Contenido)
                .NotEmpty()
                .WithMessage("El mensaje no puede estar vacío.")
                .MaximumLength(500);

            RuleFor(x => x.IdReceptor)
                .GreaterThan(0);
        }
    }
}
