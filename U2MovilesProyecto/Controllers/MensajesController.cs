using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using U2MovilesProyecto.Models.DTOs;
using U2MovilesProyecto.Services;

namespace U2MovilesProyecto.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class MensajesController : ControllerBase
    {
        private readonly MensajesService service;
        private readonly IWebHostEnvironment env;
        private readonly IValidator<MandarMensajeDTO> validator;

        public MensajesController(IWebHostEnvironment env,
            MensajesService service,
            IValidator<MandarMensajeDTO> validator)
        {
            this.service = service;
            this.validator = validator;
            this.env = env;
        }

        [HttpPost]
        public async Task<IActionResult> Enviar(MandarMensajeDTO dto)
        {
            var result = validator.Validate(dto);

            if (!result.IsValid)
                return BadRequest(result.Errors);

            try
            {
                await service.EnviarMensaje(dto);
                return Ok("Mensaje enviado.");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("{idUsuario}")]
        public IActionResult GetChat(int idUsuario)
        {
            return Ok(service.ObtenerChat(idUsuario));
        }
        [HttpPost("imagen")]
        public IActionResult EnviarImagen(EnviarImagenDTO dto)
        {
            try
            {
                var uploadsPath = Path.Combine(env.WebRootPath, "Mensajes");
                service.EnviarImagen(dto, uploadsPath);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
