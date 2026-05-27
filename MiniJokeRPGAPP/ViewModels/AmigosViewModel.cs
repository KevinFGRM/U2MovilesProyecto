using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MiniJokeRPGAPP.Models.DTOs;
using System;
using System.Collections.Generic;
using System.Text;
using MiniJokeRPGAPP.Services;
using System.Collections.ObjectModel;


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

                Usuarios.Clear();

                var lista = await amigosService.GetUsuarios();

                foreach (var item in lista)
                {
                    Usuarios.Add(item);
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
        public async Task CargarAmigos()
        {
            try
            {
                IsBusy = true;

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

                Usuarios.Clear();

                var lista = await amigosService.GetPendientes();

                foreach (var item in lista)
                {
                    Usuarios.Add(item);
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
        public async Task AgregarAmigo(UsuarioResponseDTO usuario)
        {
            try
            {
                AgregarAmigoDTO dto = new()
                {
                    NombreUsuario = usuario.NombreUsuario
                };

                await amigosService.AgregarAmigo(dto);

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
                AceptarAmigoDTO dto = new()
                {
                    IdUsuario = usuario.IdUsuario
                };

                await amigosService.AceptarAmigo(dto);

                await CargarPendientes();
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
                CrearPartidaDto dto = new()
                {
                    IdJugador2 = amigo.IdUsuario
                };

                await partidasService.CrearPartida(dto);

                // el toast
                MensajeError = "Partida creada exitosamente.";

                MenuVM.VistaActual = "Partidas";

            }
            catch (Exception ex)
            {
                MensajeError = ex.Message;
            }
        }

        [RelayCommand]
        public async Task AbrirChat(UsuarioResponseDTO usuario)
        {
            MenuVM.MensajesVM.IdUsuario = usuario.IdUsuario;

            await MenuVM.MensajesVM.CargarMensajes();

            MenuVM.VistaActual = "Chat";
        }
    }
}