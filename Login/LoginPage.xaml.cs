using OnboardingSystem.ViewModels;
using OnboardingSystem.Authentication;

namespace OnboardingSystem
{
    public partial class LoginPage : ContentPage
    {
        public LoginPage()
        {
            InitializeComponent();
            BindingContext = new LoginViewModel();
        }
    }
}