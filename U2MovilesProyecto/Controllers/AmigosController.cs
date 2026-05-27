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

        public AmigosController( AmigosService service)
        {
            this.service = service;
        }

        [HttpPost]
        public IActionResult Agregar(AgregarAmigoDTO dto)
        {
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
        public IActionResult GetAmigos()
        {
            return Ok(service.ObtenerAmigos());
        }

        [HttpGet("allusuarios")]
        public IActionResult GetUsuarios()
        {
            return Ok(service.ObtenerUsuarios());
        }

        [HttpGet("pendientes")]
        public IActionResult Pendientes()
        {
            return Ok(service.ObtenerPendientes());
        }
    }
}
