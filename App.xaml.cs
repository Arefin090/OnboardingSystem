using OnboardingSystem.Global.Menu;

namespace OnboardingSystem
{
    public partial class App : Application
    {
       
        public App()
        {
            
            MainPage = new AppShell();
            GoToInitPage();
            InitializeComponent();

        }
        private async void GoToInitPage()
        {
            await Shell.Current.GoToAsync($"//{nameof(LoginPage)}");
        }
    }
}