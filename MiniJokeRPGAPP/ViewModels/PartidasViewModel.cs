using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MiniJokeRPGAPP.Models.DTOs;
using MiniJokeRPGAPP.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace MiniJokeRPGAPP.ViewModels
{
    public partial class PartidasViewModel : ObservableObject
    {

        [ObservableProperty]
        bool isBusy;

        [ObservableProperty]
        string mensajeError = "";

        private readonly PartidasService partidasService;
        private readonly PersonajesService personajesService;
        public ObservableCollection<PartidaResponseDTO> Partidas { get; set; } = new ObservableCollection<PartidaResponseDTO>();
        public ObservableCollection<PersonajeResponseDto> Personajes { get; set; } = new ObservableCollection<PersonajeResponseDto>();
        public ObservableCollection<HabilidadResponseDto> Habilidades { get; set; } = new ObservableCollection<HabilidadResponseDto>();
        public ObservableCollection<AccionResponseDTO> Acciones { get; set; } = new ObservableCollection<AccionResponseDTO>();

        [ObservableProperty]
        int idPartida;

        [ObservableProperty]
        int idPersonaje;

        [ObservableProperty]
        string ganadorTexto;

        [ObservableProperty]
        bool plantilla;

        [ObservableProperty]
        EstadoPartidaDto? estadoPartida;

        [ObservableProperty]
        PersonajeResponseDto? personajeSeleccionado;

        public MenuViewModel MenuVM { get; set; } = null!;

        public PartidasViewModel(PartidasService partidasService, PersonajesService personajesService)
        {
            this.partidasService = partidasService;
            this.personajesService = personajesService;

        }

        [RelayCommand]
        public async Task CerrarPartida()
        {
            await MenuVM.CambiarVista("Partidas");
        }
        public async Task CargarPartidas()
        {
            try
            {
                IsBusy = true;

                Partidas.Clear();

                var lista = await partidasService.GetPartidas();

                foreach (var item in lista)
                {
                    item.Estado = item.Estado == "esperandopersonajes" ? "Esperando Personajes" : item.Estado == "activa" ? "Activa" : item.Estado == "finalizada" ? "Finalizada" : item.Estado;
                    Partidas.Add(item);
                }
                
            }
            catch (Exception ex)
            {
                MensajeError = ex.Message;
            }
            finally
            {
                IsBusy = false;
            }
        }

        [RelayCommand]
        public async Task EntrarAPartida(PartidaResponseDTO partida)
        {
            try
            {
                IsBusy = true;
                var response = await partidasService.EntrarPartida(partida.IdPartida);
                if (response != null)
                {
                    if (response.JugadorActualEligio && !response.OponenteEligio)
                    {
                        Plantilla = true;
                        EstadoPartida = null;
                        GanadorTexto = "";
                        Acciones.Clear();
                        MenuVM.VistaActual = "Juego";

                    }
                    else if(!response.JugadorActualEligio)
                    {
                        MenuVM.VistaActual = "Personajes";
                        IdPartida = response.IdPartida;
                        await CargarPersonajes();
                    }
                    else if(response.JugadorActualEligio && response.OponenteEligio)
                    {
                        Plantilla = false;
                        MenuVM.VistaActual = "Juego";
                        IdPartida = partida.IdPartida;
                        await CargarEstado();
                    }

                }

            }
            catch (Exception ex)
            {
                MensajeError = ex.Message;
            }
            finally
            {
                IsBusy = false;
            }
        }

        [RelayCommand]
        public async Task CargarPersonajes()
        {
            try
            {
                Personajes.Clear();

                var lista = await personajesService.ObtenerPersonajes();

                foreach (var item in lista)
                {
                    IdPersonaje = item.IdPersonaje;

                    var habs = await CargarHabilidades();
                    item.Habilidades = habs;
                    Personajes.Add(item);
                }
            }
            catch (Exception ex)
            {
                MensajeError = ex.Message;
            }
        }

        [RelayCommand]
        public async Task SeleccionarPersonaje(PersonajeResponseDto personajeSeleccionado)
        {
            try
            {
                if (personajeSeleccionado == null)
                    return;

                SeleccionarPersonajeDto dto = new()
                {
                    IdPartida = IdPartida,
                    IdPersonaje = personajeSeleccionado.IdPersonaje
                };

                await partidasService.SeleccionarPersonaje(dto);

                //await Shell.Current.GoToAsync("//Partida"); // Navega a pantalla de partida
                Plantilla = true;
                EstadoPartida = null;
                GanadorTexto = "";
                Acciones.Clear();
                MenuVM.VistaActual = "Juego";

            }
            catch (Exception ex)
            {
                MensajeError = ex.Message;
            }
        }

        [RelayCommand]
        public async Task CargarCombate()
        {
            try
            {
                await CargarAcciones();
                var lista = await CargarHabilidades();
                foreach (var item in lista)
                {
                    Habilidades.Add(item);
                }
            }
            catch (Exception ex)
            {
                MensajeError = ex.Message;
            }
        }

        [RelayCommand]
        public async Task CargarAcciones()
        {
            Acciones.Clear();

            var lista = await partidasService.CargarAcciones(IdPartida);

            foreach (var item in lista)
            {
                Acciones.Add(item);
            }
        }

        [RelayCommand]
        public async Task<List<HabilidadResponseDto>> CargarHabilidades()
        {
            Habilidades.Clear();

            var lista = await personajesService.GetHabilidades(IdPersonaje);

            return lista;
        }

        [RelayCommand]
        public async Task UsarHabilidad(HabilidadResponseDto habilidad)
        {
            try
            {
                RealizarAccionDto dto = new()
                {
                    IdPartida = IdPartida,
                    IdHabilidad = habilidad.IdHabilidad
                };

                await partidasService.RealizarAccion(dto);
                // Recargar historial actualizado
                await CargarEstado();
            }
            catch (Exception ex)
            {
                MensajeError = ex.Message;
            }
        }
        [RelayCommand]
        public async Task Descansar(HabilidadResponseDto habilidad)
        {
            try
            {
                RealizarAccionDto dto = new()
                {
                    IdPartida = IdPartida,
                    IdHabilidad = 0
                };

                await partidasService.RealizarAccion(dto);
                // Recargar historial actualizado
                await CargarEstado();
            }
            catch (Exception ex)
            {
                MensajeError = ex.Message;
            }

        }
        [RelayCommand]
        public async Task CargarEstado()
        {
            var estado = await partidasService.ObtenerEstado(IdPartida);
            EstadoPartida = estado;
            if(EstadoPartida != null && EstadoPartida.Estado == "finalizada")
            {
                GanadorTexto = $"Gano {EstadoPartida.Ganador}, un experto en combate.";
                return;
            }
            
            await CargarAcciones();
        }
    }
}
