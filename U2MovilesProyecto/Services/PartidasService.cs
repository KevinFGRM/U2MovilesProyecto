using AutoMapper;
using AvisosAPI.Repositories;
using U2MovilesProyecto.Models.DTOs;
using U2MovilesProyecto.Models.Entities;

namespace U2MovilesProyecto.Services
{
    public class PartidasService
    {

        private readonly Repository<Partidas> partidasRepository;
        private readonly Repository<Partidapersonajes> partidaPersonajesRepository;
        private readonly Repository<Personajes> personajesRepository;
        private readonly Repository<Habilidades> habilidadesRepository;
        private readonly Repository<Accionespartida> accionesRepository;
        private readonly Repository<Amigos> amigosRepository;

        private readonly IHttpContextAccessor httpContextAccessor;
        private readonly IMapper mapper;

        public PartidasService(
            Repository<Partidas> partidasRepository,
            Repository<Partidapersonajes> partidaPersonajesRepository,
            Repository<Personajes> personajesRepository,
            Repository<Habilidades> habilidadesRepository,
            Repository<Accionespartida> accionesRepository,
            Repository<Amigos> amigosRepository,
            IHttpContextAccessor httpContextAccessor, IMapper mapper)
        {
            this.partidasRepository = partidasRepository;
            this.partidaPersonajesRepository = partidaPersonajesRepository;
            this.personajesRepository = personajesRepository;
            this.habilidadesRepository = habilidadesRepository;
            this.accionesRepository = accionesRepository;
            this.amigosRepository = amigosRepository;
            this.httpContextAccessor = httpContextAccessor;
            this.mapper = mapper;
        }

        private int ObtenerUsuario()
        {
            return int.Parse(
                httpContextAccessor.HttpContext!
                .User.FindFirst("Id")!.Value);
        }

        public void CrearPartida(CrearPartidaDto dto)
        {
            int usuario = ObtenerUsuario();

            bool sonAmigos = amigosRepository.Query().Any(x => ((x.Usuario1 == usuario && x.Usuario2 == dto.IdJugador2) || 
            (x.Usuario1 == dto.IdJugador2 && x.Usuario2 == usuario)) && x.Estado == "aceptado");

            if (!sonAmigos)
                throw new Exception("No son amigos.");

            bool existe = partidasRepository.Query().Any(x => ((x.Jugador1 == usuario && x.Jugador2 == dto.IdJugador2) ||
                    (x.Jugador1 == dto.IdJugador2 && x.Jugador2 == usuario)) && x.Estado != "finalizada");

            if (existe)
                throw new Exception("Ya existe una partida activa.");

            Partidas partida = new()
            {
                Jugador1 = usuario,
                Jugador2 = dto.IdJugador2,
                Estado = "esperandopersonajes",
                TurnoActual = usuario
            };

            partidasRepository.Insert(partida);
        }

        public void SeleccionarPersonaje(SeleccionarPersonajeDto dto)
        {
            int usuario = ObtenerUsuario();

            var partida = partidasRepository.Get(dto.IdPartida);

            if (partida == null)
                throw new Exception("Partida no encontrada.");

            //bool pertenece = partida.Jugador1 == usuario || partida.Jugador2 == usuario;
            //if (!pertenece)
            //    throw new Exception("No perteneces a esta partida.");

            bool yaSelecciono =
                partidaPersonajesRepository.Query().Any(x => x.IdPartida == dto.IdPartida && x.IdUsuario == usuario);

            if (yaSelecciono)
                throw new Exception("Ya seleccionaste personaje.");

            var personaje = personajesRepository.Get(dto.IdPersonaje);

            if (personaje == null)
                throw new Exception("Personaje no encontrado.");

            Partidapersonajes pp = new()
            {
                IdPartida = dto.IdPartida,
                IdUsuario = usuario,
                IdPersonaje = personaje.IdPersonaje,
                VidaActual = personaje.VidaBase,
                ManaActual = personaje.ManaBase
            };

            partidaPersonajesRepository.Insert(pp);

            IniciarPartida(dto.IdPartida);
        }

        public List<PartidaResponseDTO> ObtenerPartidas()
        {
            int usuario = ObtenerUsuario();

            var partidas = partidasRepository.Query()
                .Where(x => x.Jugador1 == usuario || x.Jugador2 == usuario).OrderByDescending(x => x.FechaInicio).ToList();

            return partidas.Select(p => mapper.Map<PartidaResponseDTO>(p)).ToList();
        }

        public PartidaResponseDTO EntrarAPartida(int idPartida)
        {
            int usuario = ObtenerUsuario();

            var partida = partidasRepository.Get(idPartida);

            if (partida == null)
                throw new Exception("Partida no encontrada.");

            //bool pertenece = partida.Jugador1 == usuario || partida.Jugador2 == usuario;
            //if (!pertenece)
            //    throw new Exception("No perteneces.");

            return mapper.Map<PartidaResponseDTO>(partida);
        }

        // iniciar la partida cunado los 2 personajes esten elegidos
        public void IniciarPartida(int idPartida)
        {
            var partida = partidasRepository.Get(idPartida);

            if (partida == null)
                return;

            int cantidad =
                partidaPersonajesRepository.Query()
                .Count(x => x.IdPartida == idPartida);

            if (cantidad >= 2)
            {
                partida.Estado = "activa";

                partida.TurnoActual =
                    Random.Shared.Next(0, 2) == 0
                    ? partida.Jugador1
                    : partida.Jugador2;

                partidasRepository.Update(partida);
            }
        }

        public void RealizarAccion(RealizarAccionDto dto)
        {
            int usuario = ObtenerUsuario();

            var partida = partidasRepository.Get(dto.IdPartida);

            if (partida == null)
                throw new Exception("Partida no encontrada.");

            if (partida.Estado != "activa")
                throw new Exception("La partida no está activa.");

            if (partida.TurnoActual != usuario)
                throw new Exception("No es tu turno.");

            var personaje = partidaPersonajesRepository.Query()
                .First(x => x.IdPartida == dto.IdPartida && x.IdUsuario == usuario);

            var habilidad = habilidadesRepository.Get(dto.IdHabilidad);

            if (habilidad == null)
                throw new Exception("Habilidad no encontrada.");

            if (habilidad.IdPersonaje != personaje.IdPersonaje)
                throw new Exception("La habilidad no pertenece al personaje.");

            if (personaje.ManaActual < habilidad.CostoMana)
                throw new Exception("Mana insuficiente.");

            int enemigo = partida.Jugador1 == usuario ? partida.Jugador2 : partida.Jugador1;

            var objetivo = partidaPersonajesRepository.Query()
                .First(x => x.IdPartida == dto.IdPartida && x.IdUsuario == enemigo);


            // cambiar los nulos a 0 para evitar errores
            personaje.ManaActual -= habilidad.CostoMana ?? 0;

            objetivo.VidaActual -= habilidad.Dano ?? 0;

            if (habilidad.Curacion > 0)
            {
                personaje.VidaActual += habilidad.Curacion ?? 0;
            }

            partidaPersonajesRepository.Update(personaje);
            partidaPersonajesRepository.Update(objetivo);

            Accionespartida accion = new()
            {
                IdPartida = dto.IdPartida,
                Usuario = usuario,
                Habilidad = habilidad.IdHabilidad,
                Descripcion =
                    $"Usó {habilidad.Nombre} e hizo {habilidad.Dano} daño.",
                Fecha = DateTime.Now
            };

            accionesRepository.Insert(accion);

            if (objetivo.VidaActual <= 0)
            {
                FinalizarPartida(dto.IdPartida, usuario);
                return;
            }

            partida.TurnoActual = enemigo;

            partidasRepository.Update(partida);
        }

        public List<AccionResponseDTO> CargarAcciones(int idPartida)
        {
            var acciones = accionesRepository.Query() .Where(x => x.IdPartida == idPartida)
                .OrderBy(x => x.Fecha) .ToList();

            return acciones.Select(a => mapper.Map<AccionResponseDTO>(a)).ToList();
        }

        public void FinalizarPartida( int idPartida, int ganador)
        {
            var partida = partidasRepository.Get(idPartida);

            if (partida == null)
                return;

            partida.Estado = "finalizada";

            partida.Ganador = ganador;

            partidasRepository.Update(partida);
        }

        public List<PersonajeResponseDto> CargarPersonajes()
        {
            var personajes = personajesRepository.Query()
                .OrderBy(x => x.Nombre)
                .ToList();

            return personajes.Select(p => mapper.Map<PersonajeResponseDto>(p)).ToList();
        }
        public List<HabilidadResponseDto> CargarHabilidades(int idPersonaje)
        {
            var habilidades = habilidadesRepository.Query()
                .Where(x => x.IdPersonaje == idPersonaje)
                .ToList();

            return habilidades.Select(h=> mapper.Map<HabilidadResponseDto>(h)).ToList();
        }
    }
}
