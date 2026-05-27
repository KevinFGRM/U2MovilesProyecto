using MiniJokeRPGAPP.Views;

namespace MiniJokeRPGAPP
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();

            Routing.RegisterRoute(nameof(RegisterPage), typeof(RegisterPage));
            Routing.RegisterRoute(nameof(MenuPage), typeof(MenuPage));
            Routing.RegisterRoute(nameof(JuegoPage), typeof(JuegoPage));
            Routing.RegisterRoute(nameof(PersonajesPage), typeof(PersonajesPage));
            Routing.RegisterRoute(nameof(MensajesPage), typeof(MensajesPage));
        }
    }
}
