using Microsoft.Maui.Controls.Handlers.Compatibility;
using OnboardingSystem.Management;
using OnboardingSystem.Models.Menu;

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
    // private void LoadMenuItems() {
    //     var flyoutItems = new FlyoutItem()
    //     {
    //         Title = "Main Page",
    //         // Route = $"//{nameof(LoginPage)}",
    //         FlyoutDisplayOptions = FlyoutDisplayOptions.AsMultipleItems,
    //     };
    //     flyoutItems.Items.Add(_defaultShellItem[0]);
    //     flyoutItems.Items.Add(_defaultShellItem[1]);
    //     List<string> menuItems = MenuInitializer.menuItems;
    //     foreach(var item in menuItems) {
    //         var content = new ShellContent(){
    //             Title = item.Title,
    //             Route = item.TableName,
    //             ContentTemplate = new DataTemplate(typeof(ManagementPage)),
    //             Icon = item.Icon
    //         };
    //         flyoutItems.Items.Add(content);
    //     }
    //     flyoutItems.Items.Add(_defaultShellItem[2]);
    //     Items.Add(flyoutItems);
    // }

    private void LoadMenuItems() {
        var flyoutItems = new FlyoutItem() {
            Title = "Main Page",
            FlyoutDisplayOptions = FlyoutDisplayOptions.AsMultipleItems,
        };

        // Add the default shell items
        flyoutItems.Items.Add(_defaultShellItem[0]);
        flyoutItems.Items.Add(_defaultShellItem[1]);

        // Retrieve the menu items from the static class
        List<AppShellItem> menuItems = MenuInitializer.menuItems; // Corrected type

        foreach (var item in menuItems) {
            var content = new ShellContent() {
                Title = item.Title,             // Using AppShellItem's Title
                Route = item.TableName,         // Using TableName as Route
                ContentTemplate = new DataTemplate(typeof(ManagementPage)),
                Icon = item.Icon                // Using the Icon from AppShellItem
            };

            flyoutItems.Items.Add(content);
        }

        // Add the last default shell item
        flyoutItems.Items.Add(_defaultShellItem[2]);

        // Add the FlyoutItems to the Shell
        Items.Add(flyoutItems);
    }

}