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
    public partial class AmigosViewModel : ObservableObject
    {
        [ObservableProperty]
        bool isBusy;

        [ObservableProperty]
        string mensajeError = "";

        private readonly AmigosService amigosService;
        private readonly PartidasService partidasService;

        [ObservableProperty]
        string nombreUsuario = string.Empty;

        public ObservableCollection<AmigoResponseDTO> Friends { get; set; } = new ObservableCollection<AmigoResponseDTO>();

        public AmigosViewModel(AmigosService amigosService, PartidasService partidasService)
        {
            this.amigosService = amigosService;
            this.partidasService = partidasService;
        }

        [RelayCommand]
        public async Task CargarAmigos()
        {
            try
            {
                IsBusy = true;

                Friends.Clear();

                var lista = await amigosService.GetAmigos();

                foreach (var item in lista)
                {
                    Friends.Add(item);
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
        public async Task AgregarAmigo()
        {
            try
            {
                AgregarAmigoDTO dto = new()
                {
                    NombreUsuario = NombreUsuario
                };

                await amigosService.AgregarAmigo(dto);

                NombreUsuario = string.Empty;

                await CargarAmigos();
            }
            catch (Exception ex)
            {
                MensajeError = ex.Message;
            }
        }

        [RelayCommand]
        public async Task CrearPartidaAtAmigo(AmigoResponseDTO amigo)
        {
            try
            {
                CrearPartidaDto dto = new()
                {
                    IdJugador2 = amigo.IdUsuario
                };

                await partidasService.CrearPartida(dto);
            }
            catch (Exception ex)
            {
                MensajeError = ex.Message;
            }
        }
    }
}
