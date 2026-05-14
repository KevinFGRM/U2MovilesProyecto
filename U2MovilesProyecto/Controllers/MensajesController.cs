using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
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
        private readonly IValidator<MandarMensajeDTO> validator;

        public MensajesController(
            MensajesService service,
            IValidator<MandarMensajeDTO> validator)
        {
            this.service = service;
            this.validator = validator;
        }

        [HttpPost]
        public IActionResult Enviar(MandarMensajeDTO dto)
        {
            var result = validator.Validate(dto);

            if (!result.IsValid)
                return BadRequest(result.Errors);

            try
            {
                service.EnviarMensaje(dto);
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
    }
}
