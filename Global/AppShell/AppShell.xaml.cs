using OnboardingSystem.Models;

namespace OnboardingSystem.Global.Menu;

using Microsoft.Maui.Controls;

public partial class AppShell : Shell
{
    private ShellContent[] _shellItems = new[]
    {
        new ShellContent()
            { Title = "Dashboard", ContentTemplate = new DataTemplate(typeof(MainPage)), Icon = "dashboard_96dp_icon.png" },
        new ShellContent()
            { Title = "Profile", ContentTemplate = new DataTemplate(typeof(MainPage)), Icon = "group_96dp_icon.png" },
        new ShellContent()
            { Title = "Log Out", ContentTemplate = new DataTemplate(typeof(MainPage)), Icon = "logout_96dp_icon.png" }
    };
    public AppShell()
    {
        InitializeComponent();
        var flyoutItems = new FlyoutItem()
        {
            Title = "Main Page",
            Route = "MainPage",
            FlyoutDisplayOptions = FlyoutDisplayOptions.AsMultipleItems,
        };
        foreach (var item in _shellItems)
        {
            flyoutItems.Items.Add(item);
        }
        Items.Add(flyoutItems);
    }
}