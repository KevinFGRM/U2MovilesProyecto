#if ANDROID
using Android.Widget;
#endif
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
// solo android
#if ANDROID
using Google.Android.Material.Snackbar;
#endif
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

        public ObservableCollection<UsuarioResponseDTO> Usuarios { get; set; }
            = new ObservableCollection<UsuarioResponseDTO>();

        [ObservableProperty]
        string nombreUsuario = string.Empty;

        [ObservableProperty]
        string pestañaSeleccionada = "Amigos";

        public MenuViewModel MenuVM { get; set; } = null!;

        
        public AmigosViewModel(
            AmigosService amigosService,
            PartidasService partidasService)
        {
            this.amigosService = amigosService;
            this.partidasService = partidasService;

        }

        [RelayCommand]
        public async Task CargarUsuarios()
        {
            try
            {
                IsBusy = true;
                MensajeError = "";

                Usuarios.Clear();

                var lista = await amigosService.GetUsuarios();

                foreach (var item in lista)
                {
                    Usuarios.Add(item);
                }
                PestañaSeleccionada = "Todos";
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
        public async Task CargarAmigos()
        {
            try
            {
                IsBusy = true;
                MensajeError = "";

                Usuarios.Clear();

                var lista = await amigosService.GetAmigos();

                foreach (var item in lista)
                {
                    Usuarios.Add(new UsuarioResponseDTO
                    {
                        IdUsuario = item.IdUsuario,
                        NombreUsuario = item.NombreUsuario,
                        EstadoAmistad = "aceptado"
                    });
                }
                PestañaSeleccionada = "Amigos";
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
        public async Task CargarPendientes()
        {
            try
            {
                IsBusy = true;
                MensajeError = "";

                Usuarios.Clear();

                var lista = await amigosService.GetPendientes();

                foreach (var item in lista)
                {
                    Usuarios.Add(item);
                }
                PestañaSeleccionada = "Pendientes";
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
        public async Task AgregarAmigo(UsuarioResponseDTO usuario)
        {
            try
            {
                MensajeError = "";

                AgregarAmigoDTO dto = new()
                {
                    NombreUsuario = usuario.NombreUsuario
                };

                await amigosService.AgregarAmigo(dto);
// Codigo generado con IA indicandole que implemente snackbar solamente para android
#if ANDROID

                var activity = Platform.CurrentActivity;
                var view = activity?.FindViewById(Android.Resource.Id.Content);

                if (view != null)
                {
                    Snackbar.Make(view, "Amigo Agregado con exito", Snackbar.LengthShort)
                            .SetAction("OK", v => { })
                            .Show();
                }
#endif
                await CargarUsuarios();

            }
            catch (Exception ex)
            {
                MensajeError = ex.Message;
            }
        }

        [RelayCommand]
        public async Task AceptarAmigo(UsuarioResponseDTO usuario)
        {
            try
            {
                MensajeError = "";

                AceptarAmigoDTO dto = new()
                {
                    IdUsuario = usuario.IdUsuario
                };

                await amigosService.AceptarAmigo(dto);

                if(PestañaSeleccionada == "Todos")
                {
                    await CargarUsuarios();
                }
                else
                {
                    await CargarPendientes();
                }
            }
            catch (Exception ex)
            {
                MensajeError = ex.Message;
            }
        }

        [RelayCommand]
        public async Task CrearPartida(UsuarioResponseDTO amigo)
        {
            try
            {
                MensajeError = "";

                CrearPartidaDto dto = new()
                {
                    IdJugador2 = amigo.IdUsuario
                };

                await partidasService.CrearPartida(dto);

                // el toast
#if ANDROID
                // Código específico para Android
                var activity = Platform.CurrentActivity;
                if (activity != null)
                {
                    Toast.MakeText(activity, "Partida creada exitosamente.", ToastLength.Short).Show();
                }
#endif
                MensajeError = "Partida creada exitosamente.";

                await MenuVM.CambiarVista("Partidas");

            }
            catch (Exception ex)
            {
                MensajeError = ex.Message;
            }
        }

        [RelayCommand]
        public async Task AbrirChat(UsuarioResponseDTO usuario)
        {
            MensajeError = "";

            MenuVM.MensajesVM.IdUsuario = usuario.IdUsuario;

            await MenuVM.MensajesVM.CargarMensajes();

            MenuVM.VistaActual = "Chat";
        }
    }
}