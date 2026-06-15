using AutoMapper;
using AvisosAPI.Repositories;
using Microsoft.EntityFrameworkCore;
using MiniJokeRPGAPI.Models.Entities;
using U2MovilesProyecto.Models.DTOs;

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
        private readonly Repository<Partidahabilidades> partidaHabilidadesRepository;
        private readonly Repository<Efectospartida> efectosRepository;
        private readonly NotificacionesService notificacionesService;
        private readonly Random random = new();

        private readonly IHttpContextAccessor httpContextAccessor;
        private readonly IMapper mapper;

        public PartidasService(
            Repository<Partidas> partidasRepository,
            Repository<Partidapersonajes> partidaPersonajesRepository,
            Repository<Personajes> personajesRepository,
            Repository<Habilidades> habilidadesRepository,
            Repository<Accionespartida> accionesRepository,
            Repository<Amigos> amigosRepository,
            Repository<Partidahabilidades> partidaHabilidadesRepository,
            Repository<Efectospartida> efectosRepository,
            NotificacionesService notificacionesService,
            IHttpContextAccessor httpContextAccessor, IMapper mapper)
        {
            this.partidasRepository = partidasRepository;
            this.partidaPersonajesRepository = partidaPersonajesRepository;
            this.personajesRepository = personajesRepository;
            this.habilidadesRepository = habilidadesRepository;
            this.accionesRepository = accionesRepository;
            this.amigosRepository = amigosRepository;
            this.httpContextAccessor = httpContextAccessor;
            this.notificacionesService = notificacionesService;
            this.partidaHabilidadesRepository = partidaHabilidadesRepository;
            this.efectosRepository = efectosRepository;
            this.notificacionesService = notificacionesService;
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
            notificacionesService.EnviarNotificacion(dto.IdJugador2, 
                "Nuevo reto",
                "Un jugador te ha retado"
            );
            partidasRepository.Insert(partida);
        }

        // agregar rama
        public void SeleccionarPersonaje(SeleccionarPersonajeDto dto)
        {
            int usuario = ObtenerUsuario();

            var partida = partidasRepository.Get(dto.IdPartida);

            if (partida == null)
                throw new Exception("Partida no encontrada.");

            bool yaSelecciono = partidaPersonajesRepository.Query().Any(x => x.IdPartida == dto.IdPartida && x.IdUsuario == usuario);

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

            var partidas = partidasRepository.Query().Include(x => x.Jugador1Navigation).Include(x => x.Jugador2Navigation)
                .Where(x => x.Jugador1 == usuario || x.Jugador2 == usuario).OrderByDescending(x => x.FechaInicio).ToList();

            var response = partidas.Select(p =>
            {
                var dto = mapper.Map<PartidaResponseDTO>(p);
                dto.Jugador1 = p.Jugador1 == usuario ? $"{p.Jugador1Navigation.NombreUsuario} (Tú)" : p.Jugador1Navigation.NombreUsuario;
                dto.Jugador2 = p.Jugador2 == usuario ? $"{p.Jugador2Navigation.NombreUsuario} (Tú)" : p.Jugador2Navigation.NombreUsuario;
                if (p.Ganador != null)
                {
                    dto.Ganador = p.Ganador == p.Jugador1 ? p.Jugador1Navigation.NombreUsuario
                            : p.Jugador2Navigation.NombreUsuario;
                }
                return dto;
            }).ToList();

            return response;
        }

        public PartidaResponseDTO EntrarAPartida(int idPartida)
        {
            int usuario = ObtenerUsuario();

            var partida = partidasRepository.Query().Include(x => x.Partidapersonajes).Include(x => x.Jugador1Navigation).Include(x => x.Jugador2Navigation).FirstOrDefault(x => x.IdPartida == idPartida);

            if (partida == null)
                throw new Exception("Partida no encontrada.");

            var dto = mapper.Map<PartidaResponseDTO>(partida);
            dto.JugadorActualEligio = partida.Partidapersonajes.AsQueryable().Any(p => p.IdPartida == idPartida && p.IdUsuario == usuario && p.IdPersonaje != 0);
            dto.OponenteEligio = partida.Partidapersonajes.AsQueryable().Any(p => p.IdPartida == idPartida && p.IdUsuario != usuario && p.IdPersonaje != 0);
            return dto;
        }

        // iniciar la partida cunado los 2 personajes esten elegidos
        public void IniciarPartida(int idPartida)
        {
            var partida = partidasRepository.Get(idPartida);

            if (partida == null)
                return;

            int cantidad = partidaPersonajesRepository.Query().Count(x => x.IdPartida == idPartida);

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


            int enemigo = partida.Jugador1 == usuario ? partida.Jugador2 : partida.Jugador1;

            if (dto.IdHabilidad == 0)
            {
                partida.TurnoActual = enemigo;
                int manaRecuperado = random.Next(15, 50);

                personaje.ManaActual += manaRecuperado;

                partidaPersonajesRepository.Update(personaje);

                Accionespartida accion2 = new()
                {
                    IdPartida = dto.IdPartida,
                    Usuario = usuario,
                    Habilidad = null,
                    Descripcion =
                        $"Descanso y recupero {manaRecuperado} de mana.",
                    Fecha = DateTime.Now
                };

                accionesRepository.Insert(accion2);
                partidasRepository.Update(partida);
                return;

            }


            var habilidad = habilidadesRepository.Get(dto.IdHabilidad);

            if (habilidad == null)
                throw new Exception("Habilidad no encontrada.");


            var cartaActiva = partidaHabilidadesRepository.Query()
                .FirstOrDefault(x =>
                    x.IdPartida == dto.IdPartida &&
                    x.IdUsuario == usuario &&
                    x.IdHabilidad == dto.IdHabilidad);

            if (cartaActiva == null)
                throw new Exception("No puedes usar esa habilidad.");

            if (habilidad.IdPersonaje != personaje.IdPersonaje)
                throw new Exception("La habilidad no pertenece al personaje.");

            if (personaje.ManaActual < habilidad.CostoMana)
                throw new Exception("Mana insuficiente.");

            ProcesarInicioTurno(dto.IdPartida, personaje);  // metodo para efectos
            if (personaje.VidaActual <= 0)
            {
                FinalizarPartida(dto.IdPartida, enemigo);
                return;
            }

            var objetivo = partidaPersonajesRepository.Query()
                .First(x => x.IdPartida == dto.IdPartida && x.IdUsuario == enemigo);


            // cambiar los nulos a 0 para evitar errores
            personaje.ManaActual -= habilidad.CostoMana ?? 0;

            var (dano, esCritico) = CalcularDaño(personaje, objetivo, habilidad.Dano ?? 0, dto.IdPartida);

            objetivo.VidaActual -= dano;

            // usar efectos
            if (!string.IsNullOrEmpty(habilidad.TipoEfecto))
            {
                if (habilidad.TipoEfecto == "mana")
                {
                    personaje.ManaActual += habilidad.ValorEfecto ?? 0;

                    accionesRepository.Insert(new Accionespartida
                    {
                        IdPartida = dto.IdPartida,
                        Usuario = usuario,
                        Habilidad = habilidad.IdHabilidad,
                        Descripcion = $"{ObtenerNombrePersonaje(personaje.IdPersonaje)} recuperó {habilidad.ValorEfecto} de maná.",
                        Fecha = DateTime.Now
                    });
                }
                else if (habilidad.TipoEfecto == "robomana")
                {
                    int robo = habilidad.ValorEfecto ?? 0;

                    int roboReal = Math.Min(robo, objetivo.ManaActual);

                    objetivo.ManaActual -= roboReal;
                    personaje.ManaActual += roboReal;

                    accionesRepository.Insert(new Accionespartida
                    {
                        IdPartida = dto.IdPartida,
                        Usuario = usuario,
                        Habilidad = habilidad.IdHabilidad,
                        Descripcion = $"{ObtenerNombrePersonaje(personaje.IdPersonaje)} robó {roboReal} de maná.",
                        Fecha = DateTime.Now
                    });

                }
                else if (habilidad.TipoEfecto == "purificar")
                {
                    Purificar(dto.IdPartida, usuario);
                    accionesRepository.Insert(new Accionespartida
                    {
                        IdPartida = dto.IdPartida,
                        Usuario = usuario,
                        Habilidad = habilidad.IdHabilidad,
                        Descripcion = $"{ObtenerNombrePersonaje(personaje.IdPersonaje)} eliminó sus efectos negativos.",
                        Fecha = DateTime.Now
                    });
                }
                else
                {
                    int destino = habilidad.Objetivo == "propio" ? usuario : enemigo;
                    AplicarEfecto(dto.IdPartida, usuario, destino, habilidad);
                }
            }

            ConsumirEfectosAtaque(dto.IdPartida, usuario);
            ConsumirEfectosDefensa(dto.IdPartida, objetivo.IdUsuario);



            if (habilidad.Curacion > 0)
            {
                var datosPersonaje = personajesRepository.Get(personaje.IdPersonaje);

                personaje.VidaActual = Math.Min(personaje.VidaActual + (habilidad.Curacion ?? 0), datosPersonaje.VidaBase);

                var nombrePersonaje = ObtenerNombrePersonaje(personaje.IdPersonaje);

                accionesRepository.Insert(new Accionespartida
                {
                    IdPartida = dto.IdPartida,
                    Usuario = usuario,
                    Habilidad = habilidad.IdHabilidad,
                    Descripcion = $"{nombrePersonaje} recuperó {habilidad.Curacion} de vida.",
                    Fecha = DateTime.Now
                });
            }


            partidaPersonajesRepository.Update(personaje);
            partidaPersonajesRepository.Update(objetivo);

            if (habilidad.Curacion <= 0 && habilidad.Dano > 0)
            {
                var nombreAtacante = ObtenerNombrePersonaje(personaje.IdPersonaje);
                var nombreObjetivo = ObtenerNombrePersonaje(objetivo.IdPersonaje);
                string textoCritico = esCritico ? " ¡CRÍTICO!" : "";
                Accionespartida accion = new()
                {
                    IdPartida = dto.IdPartida,
                    Usuario = usuario,
                    Habilidad = habilidad.IdHabilidad,
                    Descripcion = $"{nombreAtacante} usó {habilidad.Nombre} e hizo {dano} de daño a {nombreObjetivo}. {textoCritico}",
                    Fecha = DateTime.Now
                };
                accionesRepository.Insert(accion);
            }

            if (objetivo.VidaActual <= 0)
            {
                FinalizarPartida(dto.IdPartida, usuario);
                return;
            }

            partidaHabilidadesRepository.Delete(cartaActiva.Id);

            var idsActuales = partidaHabilidadesRepository.Query()
                .Where(x => x.IdPartida == dto.IdPartida && x.IdUsuario == usuario).Select(x => x.IdHabilidad).ToList();


            var habilidadesDisponibles = habilidadesRepository.Query()
                .Where(x => x.IdPersonaje == personaje.IdPersonaje &&
                            !idsActuales.Contains(x.IdHabilidad)).ToList();

            var nuevaCarta = habilidadesDisponibles
                .OrderBy(x => random.Next())
                .FirstOrDefault();

            if (nuevaCarta != null)
            {
                Partidahabilidades nueva = new()
                {
                    IdPartida = dto.IdPartida,
                    IdUsuario = usuario,
                    IdHabilidad = nuevaCarta.IdHabilidad
                };

                partidaHabilidadesRepository.Insert(nueva);
            }



            partidasRepository.Update(partida);
        }
        private string ObtenerNombrePersonaje(int idPersonaje)
        {
            var personaje = personajesRepository.Get(idPersonaje);
            return personaje?.Nombre ?? $"Personaje {idPersonaje}";
        }
        // Modificaciones de efectos para las habilidades durante la partida
        private void AplicarEfecto(int idPartida, int usuario, int objetivo, Habilidades habilidad)
        {
            if (string.IsNullOrEmpty(habilidad.TipoEfecto))
                return;

            int valor = habilidad.ValorEfecto ?? 0;
            int destino = objetivo;
            if (valor < 0 && habilidad.Objetivo == "propio")
            {
                var partida = partidasRepository.Get(idPartida);
                int enemigo = partida.Jugador1 == usuario ? partida.Jugador2 : partida.Jugador1;
                destino = enemigo;
            }

            var personajeDestino = partidaPersonajesRepository.Query()
                .FirstOrDefault(x => x.IdPartida == idPartida && x.IdUsuario == destino);
            if (personajeDestino == null) return;

            string nombreDestino = ObtenerNombrePersonaje(personajeDestino.IdPersonaje);

            Efectospartida efecto = new()
            {
                IdPartida = idPartida,
                IdUsuario = destino,
                TipoEfecto = habilidad.TipoEfecto,
                Valor = valor,
                TurnosRestantes = habilidad.Duracion ?? 0
            };
            efectosRepository.Insert(efecto);

            string mensaje;
            string accion = valor >= 0 ? "aumentó" : "disminuyó";
            int valorAbs = Math.Abs(valor);

            switch (habilidad.TipoEfecto)
            {
                case "veneno":
                    mensaje = $"{nombreDestino} fue envenenado por {habilidad.Duracion} turnos.";
                    break;
                case "regeneracion":
                    mensaje = $"{nombreDestino} obtiene regeneración por {habilidad.Duracion} turnos.";
                    break;
                case "defensa":
                    mensaje = $"{nombreDestino} {accion} su defensa en {valorAbs}.";
                    break;
                case "esquiva":
                    mensaje = $"{nombreDestino} {accion} su esquiva en {valorAbs}%.";
                    break;
                case "critico":
                    mensaje = $"{nombreDestino} {accion} su probabilidad de crítico en {valorAbs}%.";
                    break;
                case "escudo":
                    mensaje = $"{nombreDestino} generó un escudo de {valorAbs}.";
                    break;
                case "congelar":
                    mensaje = $"{nombreDestino} fue congelado.";
                    break;
                case "precision":
                    mensaje = $"{nombreDestino} {accion} su precisión en {valorAbs}%.";
                    break;
                default:
                    mensaje = $"{nombreDestino} recibió el efecto: {habilidad.TipoEfecto}.";
                    break;
            }

            // El Usuario que se guarda en la acción es el caster (el que usó la habilidad)
            accionesRepository.Insert(new Accionespartida
            {
                IdPartida = idPartida,
                Usuario = usuario,
                Habilidad = habilidad.IdHabilidad,
                Descripcion = mensaje,
                Fecha = DateTime.Now
            });
        }
        private void ProcesarInicioTurno(int idPartida, Partidapersonajes personaje)
        {
            var efectos = efectosRepository.Query()
                .Where(x =>
                    x.IdPartida == idPartida &&
                    x.IdUsuario == personaje.IdUsuario)
                .ToList();

            var nombrePersonaje = ObtenerNombrePersonaje(personaje.IdPersonaje); //  obtener nombre

            foreach (var efecto in efectos)
            {
                switch (efecto.TipoEfecto)
                {
                    case "veneno":
                        personaje.VidaActual -= efecto.Valor;

                        accionesRepository.Insert(new Accionespartida
                        {
                            IdPartida = idPartida,
                            Usuario = personaje.IdUsuario,
                            Habilidad = null,
                            Descripcion = $"{nombrePersonaje} recibió {efecto.Valor} de daño por veneno.",
                            Fecha = DateTime.Now
                        });

                        efecto.TurnosRestantes--;
                        break;

                    case "regeneracion":

                        var datosPersonaje = personajesRepository.Get(personaje.IdPersonaje);

                        personaje.VidaActual = Math.Min(personaje.VidaActual + efecto.Valor, datosPersonaje.VidaBase);

                        accionesRepository.Insert(new Accionespartida
                        {
                            IdPartida = idPartida,
                            Usuario = personaje.IdUsuario,
                            Habilidad = null,
                            Descripcion = $"{nombrePersonaje} recuperó {efecto.Valor} de vida.",
                            Fecha = DateTime.Now
                        });

                        efecto.TurnosRestantes--;
                        break;

                    case "congelar":
                        accionesRepository.Insert(new Accionespartida
                        {
                            IdPartida = idPartida,
                            Usuario = personaje.IdUsuario,
                            Habilidad = null,
                            Descripcion = $"{nombrePersonaje} está congelado y esquiva más lento.",
                            Fecha = DateTime.Now
                        });

                        efecto.TurnosRestantes--;
                        break;
                }

                if (efecto.TurnosRestantes <= 0)
                    efectosRepository.Delete(efecto.IdEfecto);
                else
                    efectosRepository.Update(efecto);
            }

            partidaPersonajesRepository.Update(personaje);
        }
        private (int daño, bool esCritico) CalcularDaño(Partidapersonajes atacante, Partidapersonajes defensor, int dañoBase, int idPartida)
        {
            int daño = dañoBase;
            bool critico = false;

            var efectosAtacante = efectosRepository.Query()
                .Where(x => x.IdPartida == idPartida && x.IdUsuario == atacante.IdUsuario).ToList();

            var efectosDefensor = efectosRepository.Query()
                .Where(x => x.IdPartida == idPartida && x.IdUsuario == defensor.IdUsuario).ToList();

            int precisionAtacante = efectosAtacante.Where(x => x.TipoEfecto == "precision").Sum(x => x.Valor);

            int esquivaDefensor = efectosDefensor.Where(x => x.TipoEfecto == "esquiva").Sum(x => x.Valor);

            int probabilidadFallo = esquivaDefensor - precisionAtacante;

            probabilidadFallo = Math.Clamp(probabilidadFallo, 0, 100);

            if (random.Next(1, 101) <= probabilidadFallo)
            {
                var nombreDefensor = ObtenerNombrePersonaje(defensor.IdPersonaje);

                accionesRepository.Insert(new Accionespartida
                {
                    IdPartida = idPartida,
                    Usuario = defensor.IdUsuario,
                    Habilidad = null,
                    Descripcion = $"{nombreDefensor} esquivó el ataque.",
                    Fecha = DateTime.Now
                });

                return (0, false);
            }

            int probCritico = efectosAtacante.Where(x => x.TipoEfecto == "critico").Sum(x => x.Valor);

            if (probCritico > 0 && random.Next(1, 101) <= probCritico)
            {
                critico = true;
                daño = (int)(daño * 1.5);
            }

            bool ignorarDefensa = efectosAtacante.Any(x => x.TipoEfecto == "ignorardefensa");

            if (!ignorarDefensa)
            {
                var personajeDefensor = personajesRepository.Get(defensor.IdPersonaje);

                int defensaBase = personajeDefensor?.DefensaBase ?? 0;

                int defensaExtra = efectosDefensor.Where(x => x.TipoEfecto == "defensa") .Sum(x => x.Valor);

                int defensaTotal = defensaBase + defensaExtra;

                daño -= (int)(daño * (defensaTotal / 100.0));
            }

            var escudos = efectosDefensor.Where(x => x.TipoEfecto == "escudo").ToList();

            foreach (var escudo in escudos)
            {
                if (daño <= 0)
                    break;

                int absorbido = Math.Min(daño, escudo.Valor);

                daño -= absorbido;
                escudo.Valor -= absorbido;

                if (escudo.Valor <= 0)
                    efectosRepository.Delete(escudo.IdEfecto);
                else
                    efectosRepository.Update(escudo);
            }

            if (daño < 0)
                daño = 0;

            return (daño, critico);
        }
        private void ConsumirEfectosAtaque(int idPartida, int idUsuario)
        {
            var efectos = efectosRepository.Query()
                .Where(x => x.IdPartida == idPartida && x.IdUsuario == idUsuario).ToList();

            foreach (var efecto in efectos)
            {
                if (efecto.TipoEfecto == "critico" || efecto.TipoEfecto == "ignorardefensa" || efecto.TipoEfecto == "precision")
                {
                    efecto.TurnosRestantes--;

                    if (efecto.TurnosRestantes <= 0)
                        efectosRepository.Delete(efecto.IdEfecto);
                    else
                        efectosRepository.Update(efecto);
                }
            }
        }
        private void ConsumirEfectosDefensa(int idPartida, int idUsuario)
        {
            var efectos = efectosRepository.Query()
                .Where(x => x.IdPartida == idPartida && x.IdUsuario == idUsuario).ToList();

            foreach (var efecto in efectos)
            {
                if (efecto.TipoEfecto == "esquiva" || efecto.TipoEfecto == "defensa" || efecto.TipoEfecto == "escudo")
                {
                    efecto.TurnosRestantes--;

                    if (efecto.TurnosRestantes <= 0)
                        efectosRepository.Delete(efecto.IdEfecto);
                    else
                        efectosRepository.Update(efecto);
                }
            }
        }
        private void Purificar(int idPartida, int idUsuario)
        {

            var efectos = efectosRepository.Query()
                .Where(x => x.IdPartida == idPartida && x.IdUsuario == idUsuario &&
                    ( x.TipoEfecto == "veneno" || x.TipoEfecto == "congelar" || x.TipoEfecto == "precision")).ToList();

            foreach (var e in efectos)
                efectosRepository.Delete(e.IdEfecto);
        }

        // aqui terminan las modificaciones de efectos

        public List<AccionResponseDTO> CargarAcciones(int idPartida)
        {
            var acciones = accionesRepository.Query().Where(x => x.IdPartida == idPartida)
                .OrderBy(x => x.Fecha).ToList();

            return acciones.Select(a => mapper.Map<AccionResponseDTO>(a)).ToList();
        }

        public void FinalizarPartida(int idPartida, int ganador)
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
            var habilidades = habilidadesRepository.Query().Where(x => x.IdPersonaje == idPersonaje).ToList();

            return habilidades.Select(h => mapper.Map<HabilidadResponseDto>(h)).ToList();
        }

        public EstadoPartidaDto ObtenerEstado(int idPartida)
        {
            int usuario = ObtenerUsuario();

            var partida = partidasRepository.Query().Include(x => x.Partidapersonajes).Include(x => x.Jugador1Navigation)
                .Include(x => x.Jugador2Navigation).FirstOrDefault(x => x.IdPartida == idPartida);

            if (partida == null)
                throw new Exception("Partida no encontrada.");

            if (partida.Estado == "esperandopersonajes")
            {
                partida.Estado = "activa";
                partidasRepository.Update(partida);
            }

            bool esJugador1 = partida.Jugador1 == usuario;
            bool esJugador2 = partida.Jugador2 == usuario;

            var miPersonaje = partida.Partidapersonajes.First(x => x.IdUsuario == usuario);
            var oponentePersonaje = partida.Partidapersonajes.First(x => x.IdUsuario != usuario);

            string nombreJugador1 = partida.Jugador1Navigation.NombreUsuario;
            string nombreJugador2 = partida.Jugador2Navigation.NombreUsuario;

            if (esJugador1)
                nombreJugador1 = $"{nombreJugador1} (Tú)";
            else if (esJugador2)
                nombreJugador2 = $"{nombreJugador2} (Tú)";

            var estado = new EstadoPartidaDto
            {
                Jugador1 = esJugador1 ? $"{partida.Jugador1Navigation.NombreUsuario} (Tú)" : $"{partida.Jugador2Navigation.NombreUsuario} (Tú)",
                Jugador2 = esJugador1 ? partida.Jugador2Navigation.NombreUsuario : partida.Jugador1Navigation.NombreUsuario,

                Personaje1 = miPersonaje.IdPersonaje,
                VidaJugador1 = miPersonaje.VidaActual,
                ManaJugador1 = miPersonaje.ManaActual,

                Personaje2 = oponentePersonaje.IdPersonaje,
                VidaEnemigo2 = oponentePersonaje.VidaActual,
                ManaEnemigo2 = oponentePersonaje.ManaActual,

                MiTurno = partida.TurnoActual == usuario,
                Estado = partida.Estado,
            };
            if (estado.Estado == "finalizada")
            {
                estado.Ganador = partida.Ganador == usuario ? $"{(partida.GanadorNavigation != null ? partida.GanadorNavigation.NombreUsuario : "Tú")} (Tú)"
                              : (partida.GanadorNavigation != null ? partida.GanadorNavigation.NombreUsuario : "(Oponente)");
            }

            var habilidadesGuardadas = partidaHabilidadesRepository.Query()
                .Where(x =>
                    x.IdPartida == idPartida &&
                    x.IdUsuario == usuario)
                .Include(x => x.IdHabilidadNavigation)
                .ToList();
            if (!habilidadesGuardadas.Any())
            {
                var todasHabilidades = habilidadesRepository.Query()
                    .Where(x => x.IdPersonaje == miPersonaje.IdPersonaje)
                    .ToList();

                var randoms = todasHabilidades
                    .OrderBy(x => random.Next())
                    .Take(3)
                    .ToList();

                foreach (var habilidad in randoms)
                {
                    Partidahabilidades ph = new()
                    {
                        IdPartida = idPartida,
                        IdUsuario = usuario,
                        IdHabilidad = habilidad.IdHabilidad
                    };

                    partidaHabilidadesRepository.Insert(ph);
                }

                habilidadesGuardadas = partidaHabilidadesRepository.Query()
                    .Where(x =>
                        x.IdPartida == idPartida &&
                        x.IdUsuario == usuario)
                    .Include(x => x.IdHabilidadNavigation)
                    .ToList();
            }

            estado.HabilidadesJugador1 = habilidadesGuardadas
                .Select(x => mapper.Map<HabilidadResponseDto>(x.IdHabilidadNavigation)).ToList();

            return estado;
        }
    }
}
