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
    public class PartidasController : ControllerBase
    {
        private readonly PartidasService service;

        private readonly IValidator<CrearPartidaDto> crearValidator;
        private readonly IValidator<SeleccionarPersonajeDto> personajeValidator;

        public PartidasController(
            PartidasService service,
            IValidator<CrearPartidaDto> crearValidator,
            IValidator<SeleccionarPersonajeDto> personajeValidator)
        {
            this.service = service;
            this.crearValidator = crearValidator;
            this.personajeValidator = personajeValidator;
        }

        [HttpPost]
        public async Task<IActionResult> Crear(CrearPartidaDto dto)
        {
            var result = crearValidator.Validate(dto);

            if (!result.IsValid)
                return BadRequest(result.Errors);

            try
            {
                await service.CrearPartida(dto);

                return Ok("Partida creada.");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("personaje")]
        public IActionResult SeleccionarPersonaje(SeleccionarPersonajeDto dto)
        {
            var result = personajeValidator.Validate(dto);

            if (!result.IsValid)
                return BadRequest(result.Errors);

            try
            {
                service.SeleccionarPersonaje(dto);

                return Ok("Personaje seleccionado.");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet]
        public IActionResult ObtenerPartidas()
        {
            return Ok(service.ObtenerPartidas());
        }

        [HttpGet("{id}")]
        public IActionResult Entrar(int id)
        {
            try
            {
                var dto = service.EntrarAPartida(id);
                if(dto != null)
                {
                    return Ok(dto);
                }
                return NotFound();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("accion")]
        public IActionResult Accion(RealizarAccionDto dto)
        {
            try
            {
                service.RealizarAccion(dto);

                return Ok("Acción realizada.");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        
        [HttpGet("{id}/acciones")]
        public IActionResult Acciones(int id)
        {
            return Ok(service.CargarAcciones(id));
        }

        [HttpGet("personajes")]
        public IActionResult Personajes()
        {
            return Ok(service.CargarPersonajes());
        }
        [HttpGet("personajes/{id}/habilidades")]
        public IActionResult Habilidades(int id)
        {
            return Ok(service.CargarHabilidades(id));
        }

        [HttpGet("{idPartida}/estado")]
        public IActionResult ObtenerEstado(int idPartida)
        {
            return Ok(service.ObtenerEstado(idPartida));
        }
    }
}
