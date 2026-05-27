using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MiniJokeRPGAPP.Models.DTOs;
using MiniJokeRPGAPP.Services;
using MiniJokeRPGAPP.Views;
using System;
using System.Collections.Generic;
using System.Text;
namespace MiniJokeRPGAPP.ViewModels
{
    public partial class AuthViewModel : ObservableObject
    {

        [ObservableProperty]
        bool isBusy;

        [ObservableProperty]
        string mensajeError = "";

        private readonly AuthService authService;

        [ObservableProperty]
        string correo = "";

        [ObservableProperty]
        string contrasena = "";

        [ObservableProperty]
        string nombreUsuario = "";
        public AuthViewModel(AuthService authService)
        {
            this.authService = authService;
        }

        [RelayCommand]
        public async Task Login()
        {
            try
            {
                IsBusy = true;

                LoginDto dto = new()
                {
                    Correo = Correo,
                    Contrasena = Contrasena
                };

                var response = await authService.Login(dto);

                if (response != null)
                {
                    await Shell.Current.GoToAsync("MenuPage");
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
        public async Task IrRegistrar()
        {
            await Shell.Current.GoToAsync("RegisterPage");
        }
        [RelayCommand]
        public async Task IrLogin()
        {
            await Shell.Current.GoToAsync("///LoginPage");
        }

        [RelayCommand]
        public async Task Register()
        {
            try
            {
                IsBusy = true;

                RegisterDto dto = new()
                {
                    NombreUsuario = NombreUsuario,
                    Correo = Correo,
                    Contrasena = Contrasena
                };

                await authService.Register(dto);

                await Shell.Current.GoToAsync("///LoginPage");
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
        public void Logout()
        {
            try
            {
                IsBusy = true;
                authService.LogOut();
                Shell.Current.GoToAsync("///LoginPage");
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
    }
}
