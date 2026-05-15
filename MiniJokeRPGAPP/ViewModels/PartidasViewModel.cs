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
        PersonajeResponseDto? selectedCharacter;

        public PartidasViewModel(PartidasService partidasService, PersonajesService personajesService)
        {
            this.partidasService = partidasService;
            this.personajesService = personajesService;
        }

        [RelayCommand]
        public async Task CargarPartidas()
        {
            try
            {
                IsBusy = true;

                Partidas.Clear();

                var lista = await partidasService.GetPartidas();

                foreach (var item in lista)
                {
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
            // Navega a pantalla de batalla
            //await Shell.Current.GoToAsync($"battle?id={partida.IdPartida}");
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
                    Personajes.Add(item);
                }
            }
            catch (Exception ex)
            {
                MensajeError = ex.Message;
            }
        }

        [RelayCommand]
        public async Task SeleccionarPersonaje()
        {
            try
            {
                if (SelectedCharacter == null)
                    return;

                SeleccionarPersonajeDto dto = new()
                {
                    IdPersonaje = SelectedCharacter.IdPersonaje
                };

                await partidasService.SeleccionarPersonaje(IdPartida, dto);

                await Shell.Current.GoToAsync("//Partida"); // Navega a pantalla de partida
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
                await CargarHabilidades();
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
        public async Task CargarHabilidades()
        {
            Habilidades.Clear();

            var lista = await personajesService.GetHabilidades(IdPersonaje);

            foreach (var item in lista)
            {
                Habilidades.Add(item);
            }
        }

        [RelayCommand]
        public async Task UsarHabilidad(HabilidadResponseDto habilidad)
        {
            try
            {
                RealizarAccionDto dto = new()
                {
                    IdHabilidad = habilidad.IdHabilidad
                };

                await partidasService.RealizarAccion(IdPartida, dto);

                // Recargar historial actualizado
                await CargarAcciones();
            }
            catch (Exception ex)
            {
                MensajeError = ex.Message;
            }
        }
    }
}
