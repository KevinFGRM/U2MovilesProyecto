using MiniJokeRPGAPP.ViewModels;

namespace MiniJokeRPGAPP.Views;

public partial class LoginPage : ContentPage
{
	public LoginPage(AuthViewModel viewModel)
	{
		InitializeComponent();
		BindingContext = viewModel;
	}
}