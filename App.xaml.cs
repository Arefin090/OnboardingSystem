using OnboardingSystem.Authentication;
using OnboardingSystem.Global.Menu;

namespace OnboardingSystem
{
    public partial class App : Application
    {
       
        public App(IAuthenticationService authService)
                {
                    InitializeComponent();
                    MainPage = new AppShell(authService);
                    Shell.Current.GoToAsync($"//{nameof(LoginPage)}");
                }
    }
}