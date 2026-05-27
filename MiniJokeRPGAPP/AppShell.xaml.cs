using MiniJokeRPGAPP.Views;

namespace MiniJokeRPGAPP
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();

            Routing.RegisterRoute(nameof(RegisterPage), typeof(RegisterPage));
        }
    }
}
