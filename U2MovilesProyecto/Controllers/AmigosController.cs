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
    public class AmigosController : ControllerBase
    {
        private readonly AmigosService service;
        private readonly IValidator<AgregarAmigoDTO> validator;

        public AmigosController(
            AmigosService service,
            IValidator<AgregarAmigoDTO> validator)
        {
            this.service = service;
            this.validator = validator;
        }

        [HttpPost]
        public IActionResult Agregar(AgregarAmigoDTO dto)
        {
            var result = validator.Validate(dto);

            if (!result.IsValid)
                return BadRequest(result.Errors);

            try
            {
                service.AgregarAmigo(dto);
                return Ok("Amigo agregado.");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpPost("aceptar")]
        public IActionResult Aceptar(AceptarAmigoDTO dto)
        {
            try
            {
                service.AceptarAmigo(dto);
                return Ok("Amigo aceptado.");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpGet]
        public IActionResult GetFriends()
        {
            return Ok(service.ObtenerAmigos());
        }
    }
}
