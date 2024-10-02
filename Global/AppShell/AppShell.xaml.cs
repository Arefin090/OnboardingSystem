using Microsoft.Maui.Controls.Handlers.Compatibility;
using OnboardingSystem.Management;
using OnboardingSystem.Models.Menu;

namespace OnboardingSystem.Global.Menu;

using Microsoft.Maui.Controls;
using System.ComponentModel;
using System.Runtime.CompilerServices;

public partial class AppShell : Shell
{
    private ShellContent[] _defaultShellItem = new[]
    {
        new ShellContent() { Title = "Dashboard", ContentTemplate = new DataTemplate(typeof(DashboardPage)), Icon = "dashboard_96dp.png", Route = $"{nameof(DashboardPage)}"},
        new ShellContent() { Title = "User List", ContentTemplate = new DataTemplate(typeof(UserListPage)), Icon = "group_96dp.png", Route = $"{nameof(UserListPage)}"},
        new ShellContent() { Title = "Profile", ContentTemplate = new DataTemplate(typeof(ProfilePage)), Icon = "group_96dp.png", Route = $"{nameof(ProfilePage)}"},
        new ShellContent() { Title = "Log Out", ContentTemplate = new DataTemplate(typeof(LoginPage)), Icon = "logout_96dp.png", Route = $"{nameof(LoginPage)}"},
    };

    public event PropertyChangedEventHandler? PropertyChanged;
    private string _currentRoute;
    public string CurrentRoute
    {
        get => _currentRoute;
        set
        {
            if (_currentRoute != value)
            {
                _currentRoute = value;
                OnPropertyChanged();
            }
        }
    }

    public AppShell()
    {
        InitializeComponent();
        LoadMenuItems();
    }

    private void LoadMenuItems()
    {
        var flyoutItems = new FlyoutItem()
        {
            Title = "Main Page",
            FlyoutDisplayOptions = FlyoutDisplayOptions.AsMultipleItems,
        };

        // Adding default items
        flyoutItems.Items.Add(_defaultShellItem[0]);
        flyoutItems.Items.Add(_defaultShellItem[1]);
        flyoutItems.Items.Add(_defaultShellItem[2]);

        // Load menu items from your initializer
        var menuItems = MenuInitializer.menuItems;
        foreach (var item in menuItems)
        {
            var content = new ShellContent()
            {
                Title = item.Title,
                Route = item.TableName, // Ensure this is unique for each item
                ContentTemplate = new DataTemplate(typeof(ManagementPage)),
                Icon = item.Icon                // Using the Icon from AppShellItem
            };

            flyoutItems.Items.Add(content);
        }

        // Adding the last default item
        flyoutItems.Items.Add(_defaultShellItem[3]);

        // Finally, add the FlyoutItem to the Shell
        Items.Add(flyoutItems);
    }

private void OnNavigated(object sender, ShellNavigatedEventArgs e)
{
    if (e?.Current?.Location != null)
    {
        CurrentRoute = e.Current.Location.ToString(); // Update the current route
        // Notify the ManagementPage or any other pages that the route has changed
        MessagingCenter.Send(this, "RouteChanged", CurrentRoute);
    }
}



    protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        Shell.Current.Navigated -= OnNavigated; // Unsubscribe from the event
    }
}

