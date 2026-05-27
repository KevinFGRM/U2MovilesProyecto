using MiniJokeRPGAPP.ViewModels;

namespace MiniJokeRPGAPP.Views;

public partial class RegisterPage : ContentPage
{
	public RegisterPage(AuthViewModel viewModel)
	{
		InitializeComponent();
		BindingContext = viewModel;
	}
}