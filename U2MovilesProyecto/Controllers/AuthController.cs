using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MiniJokeRPGAPI.Models.DTOs;
using U2MovilesProyecto.Models.DTOs;
using U2MovilesProyecto.Services;

namespace U2MovilesProyecto.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        public readonly AuthService service;
        private readonly IValidator<LoginDto> validator;
        private readonly IValidator<RegisterDto> registerValidator;

        public AuthController(AuthService service, IValidator<LoginDto> validator, IValidator<RegisterDto> registerValidator)
        {
            this.service = service;
            this.validator = validator;
            this.registerValidator = registerValidator;
        }

        [HttpPost]
        public IActionResult Login(LoginDto dto)
        {
            var result = validator.Validate(dto);

            if (!result.IsValid)
                return BadRequest(result.Errors.Select(x => x.ErrorMessage));
            try
            {
                var response = service.Login(dto);
                return Ok(response);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(ex.Message);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpPost("register")]
        public IActionResult Register(RegisterDto dto)
        {
            var result = registerValidator.Validate(dto);
            if (!result.IsValid)
                return BadRequest(result.Errors.Select(x => x.ErrorMessage));
            try
            {
                service.RegistrarMaestro(dto);
                return Ok("Usuario registrado exitosamente.");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [Authorize]
        [HttpPost("fcmtoken")]
        public IActionResult GuardarToken(FcmTokenDTO dto)
        {
            service.GuardarToken(dto.Token);

            return Ok();
        }
    }
}
