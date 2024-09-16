using Microsoft.Maui.Controls.Handlers.Compatibility;
using OnboardingSystem.Management;
using OnboardingSystem.Models;

namespace OnboardingSystem.Global.Menu;

using Microsoft.Maui.Controls;

public partial class AppShell : Shell
{
    private ShellContent[] _shellItems = new[]
    {
        new ShellContent()
            { Title = "Dashboard", ContentTemplate = new DataTemplate(typeof(DashboardPage)), Icon = "dashboard_96dp_icon.png", Route = $"{nameof(DashboardPage)}"},
        new ShellContent()
            { Title = "Profile", ContentTemplate = new DataTemplate(typeof(ManagementPage)), Icon = "group_96dp_icon.png", Route = $"{nameof(ManagementPage)}"},
        new ShellContent()
            { Title = "Log Out", ContentTemplate = new DataTemplate(typeof(LoginPage)), Icon = "logout_96dp_icon.png", Route = $"{nameof(LoginPage)}"}
    };
    public  AppShell()
    {
        InitializeComponent();
        
        var flyoutItems = new FlyoutItem()
        {
            Title = "Main Page",
            // Route = $"//{nameof(LoginPage)}",
            FlyoutDisplayOptions = FlyoutDisplayOptions.AsMultipleItems,
        };
        foreach (var item in _shellItems)
        {
            flyoutItems.Items.Add(item);
        }
        
        Items.Add(flyoutItems);
    }
}