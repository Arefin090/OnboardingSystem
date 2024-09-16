using OnboardingSystem.Global.Menu;
namespace OnboardingSystem;

public partial class App : Application
{
	public App()
	{
		InitializeComponent();
		MainPage = new AppShell();
		Shell.Current.GoToAsync($"//{nameof(LoginPage)}");
	}
}
