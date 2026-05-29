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
    public partial class MensajesViewModel : ObservableObject
    {
        [ObservableProperty]
        bool isBusy;

        [ObservableProperty]
        string mensajeError = "";

        private readonly MensajesService mensajesService;
        public ObservableCollection<MensajeResponseDTO> Mensajes { get; set; } = new ObservableCollection<MensajeResponseDTO>();

        [ObservableProperty]
        string mensaje = "";

        [ObservableProperty]
        int idUsuario;

        [ObservableProperty]
        string nombreAmigo = "";
        public MensajesViewModel(MensajesService mensajesService)
        {
            this.mensajesService = mensajesService;
        }

        [RelayCommand]
        public async Task CargarMensajes()
        {
            try
            {
                Mensajes.Clear();
                var lista =
                    await mensajesService.ObtenerChat(IdUsuario);
                NombreAmigo = lista.NombreAmigo;

                foreach (var item in lista.Mensajes)
                {
                    Mensajes.Add(item);
                }
            }
            catch (Exception ex)
            {
                MensajeError = ex.Message;
            }
        }

        [RelayCommand]
        public async Task MandarMensaje()
        {
            try
            {
                if (string.IsNullOrWhiteSpace(Mensaje))
                    return;

                MandarMensajeDTO dto = new()
                {
                    IdReceptor = IdUsuario,
                    Contenido = Mensaje
                };

                await mensajesService.MandarMensaje(dto);

                Mensaje = "";

                await CargarMensajes();
            }
            catch (Exception ex)
            {
                MensajeError = ex.Message;
            }
        }

        [RelayCommand]
        public async Task SeleccionarImagen()
        {
            try
            {
                var foto = await MediaPicker.Default.PickPhotoAsync();

                if (foto == null)
                    return;

                using var stream = await foto.OpenReadAsync();

                using MemoryStream ms = new();

                await stream.CopyToAsync(ms);

                var base64 = Convert.ToBase64String(ms.ToArray());

                EnviarImagenDTO dto = new()
                {
                    Receptor = IdUsuario,
                    ImagenBase64 = base64
                };

                await mensajesService.EnviarImagen(dto);

                await CargarMensajes();
            }
            catch (Exception ex)
            {
                MensajeError = ex.Message;
            }
        }
    }
}
