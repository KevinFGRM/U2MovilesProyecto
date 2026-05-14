using AvisosAPI.Repositories;
using FluentValidation;
using U2MovilesProyecto.Models.DTOs;
using U2MovilesProyecto.Models.Entities;

namespace U2MovilesProyecto.Validators
{
    public class LoginValidator : AbstractValidator<LoginDto>
    {
        public LoginValidator()
        {
            RuleFor(x => x.Correo)
                .NotEmpty().WithMessage("El correo es requerido.")
                .EmailAddress().WithMessage("El correo no tiene un formato válido.");

            RuleFor(x => x.Contrasena)
                .NotEmpty().WithMessage("La contraseña es requerida.");
        }
    }

    public class RegisterValidator : AbstractValidator<RegisterDto>
    {
        public RegisterValidator(Repository<Usuarios> usuarioRepository)
        {
            RuleFor(x => x.NombreUsuario)
                .NotEmpty().WithMessage("El nombre de usuario es requerido.")
                .MinimumLength(3).WithMessage("El nombre de usuario debe tener al menos 3 caracteres.")
                .MaximumLength(30).WithMessage("El nombre de usuario no puede exceder 30 caracteres.")
                .Must(nombreUsuario =>
                    !usuarioRepository.Query()
                        .Any(u => u.NombreUsuario == nombreUsuario))
                .WithMessage("El nombre de usuario ya está registrado.");

            RuleFor(x => x.Correo)
                .NotEmpty().WithMessage("El correo es requerido.")
                .EmailAddress().WithMessage("El correo no tiene un formato válido.")
                .Must(correo =>
                    !usuarioRepository.Query()
                        .Any(u => u.Correo == correo))
                .WithMessage("El correo ya está registrado.");

            RuleFor(x => x.Contrasena)
                .NotEmpty().WithMessage("La contraseña es requerida.")
                .MinimumLength(6).WithMessage("La contraseña debe tener al menos 6 caracteres.");
        }
    }
}
