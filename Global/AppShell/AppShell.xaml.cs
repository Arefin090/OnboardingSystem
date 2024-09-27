using Microsoft.Maui.Controls.Handlers.Compatibility;
using OnboardingSystem.Management;
using OnboardingSystem.Models;

namespace OnboardingSystem.Global.Menu;

using Microsoft.Maui.Controls;

public partial class AppShell : Shell
{
    private ShellContent[] _defaultShellItem = new[]
    {
        new ShellContent()
            { Title = "Dashboard", ContentTemplate = new DataTemplate(typeof(DashboardPage)), Icon = "dashboard_96dp_icon.png", Route = $"{nameof(DashboardPage)}"},
		new ShellContent()
			{ Title = "User List", ContentTemplate = new DataTemplate(typeof(UserListPage)), Icon = "group_96dp_icon.png", Route = $"{nameof(UserListPage)}"},
		new ShellContent()
            { Title = "Log Out", ContentTemplate = new DataTemplate(typeof(LoginPage)), Icon = "logout_96dp_icon.png", Route = $"{nameof(LoginPage)}"},
    };
    public  AppShell()
    {
        InitializeComponent();
        
        // var flyoutItems = new FlyoutItem()
        // {
        //     Title = "Main Page",
        //     // Route = $"//{nameof(LoginPage)}",
        //     FlyoutDisplayOptions = FlyoutDisplayOptions.AsMultipleItems,
        // };
        // foreach (var item in _defaultShellItem)
        // {
        //     flyoutItems.Items.Add(item);
        // }
        // Items.Add(flyoutItems);
        LoadMenuItems();
    }
    private void LoadMenuItems() {
        var flyoutItems = new FlyoutItem()
        {
            Title = "Main Page",
            // Route = $"//{nameof(LoginPage)}",
            FlyoutDisplayOptions = FlyoutDisplayOptions.AsMultipleItems,
        };
        flyoutItems.Items.Add(_defaultShellItem[0]);
        flyoutItems.Items.Add(_defaultShellItem[1]);
        var menuItems = new MenuInitializer().menuItems;
        foreach(var item in menuItems) {
            var content = new ShellContent(){
                Title = item.Title,
                Route = item.TableName,
                ContentTemplate = new DataTemplate(typeof(ManagementPage)),
                Icon = item.Icon
            };
            flyoutItems.Items.Add(content);
        }
        flyoutItems.Items.Add(_defaultShellItem[2]);
        Items.Add(flyoutItems);
    }
}