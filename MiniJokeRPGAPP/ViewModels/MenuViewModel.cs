using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Text;

namespace MiniJokeRPGAPP.ViewModels
{
    public partial class MenuViewModel : ObservableObject
    {
        [ObservableProperty]
        bool isBusy;

        [ObservableProperty]
        string mensajeError = "";

        [ObservableProperty]
        string vistaActual = "Presentacion";

        public AmigosViewModel AmigosVM { get; }
        public MensajesViewModel MensajesVM { get; }
        public PartidasViewModel PartidasVM { get; }
        public AuthViewModel AuthVM { get; }
        public MenuViewModel(AmigosViewModel amigosViewModel, MensajesViewModel mensajesViewModel, PartidasViewModel partidasViewModel, AuthViewModel authVM)
        {
            AmigosVM = amigosViewModel;
            MensajesVM = mensajesViewModel;
            PartidasVM = partidasViewModel;
            AuthVM = authVM;

            AmigosVM.MenuVM = this;
            PartidasVM.MenuVM = this;

            VistaActual = "Presentacion";
        }
        [RelayCommand]
        public async Task CambiarVista(string vista)
        {
            // aqui podria agregar la notificacion toast o algo para indicar que se esta cambiando de vista o si no se pudo cambiar por algun error
            VistaActual = vista;
            switch (VistaActual)
            {
                case "Amigos":
                    await AmigosVM.CargarAmigos();
                    break;
                case "Partidas":
                    await PartidasVM.CargarPartidas();
                    break;


            }
        }
    }
}
