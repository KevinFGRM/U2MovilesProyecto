using MiniJokeRPGAPP.ViewModels;

namespace MiniJokeRPGAPP.Views;

public partial class MenuPage : ContentPage
{
	public MenuPage(MenuViewModel menuViewModel)
	{
		InitializeComponent();
		BindingContext = menuViewModel;
	}
}