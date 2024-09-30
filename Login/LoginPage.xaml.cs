using OnboardingSystem.ViewModels;
using OnboardingSystem.Authentication;

namespace OnboardingSystem
{
    public partial class LoginPage : ContentPage
    {
        public LoginPage(IAuthenticationService authService)
        {
            InitializeComponent();
            BindingContext = new LoginViewModel(authService);
        }
    }
}