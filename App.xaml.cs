using OnboardingSystem.Authentication;
using OnboardingSystem.Global.Menu;

namespace OnboardingSystem
{
    public partial class App : Application
    {
       
        private async void GoToInitPage()
        {
            await Shell.Current.GoToAsync($"//{nameof(LoginPage)}");
        }
        public App(IAuthenticationService authService)
                {
			//GoToInitPage();
			InitializeComponent();
                    MainPage = new AppShell();
                    Shell.Current.GoToAsync($"//{nameof(LoginPage)}");
                }
    }
}