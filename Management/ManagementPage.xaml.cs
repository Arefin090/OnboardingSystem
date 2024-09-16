
using OnboardingSystem.ViewModel;

namespace OnboardingSystem.Management;

public partial class ManagementPage : ContentPage
{
    public ManagementPage()
    {
        InitializeComponent();
        BindingContext = new ManagementViewModel();
    }
}