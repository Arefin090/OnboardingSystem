using System.Collections.ObjectModel;
using System.ComponentModel;

namespace OnboardingSystem.Global.Menu;
using Microsoft.Maui.Controls;

public partial class Menu : Shell
{
    public class MenuItemViewModel : INotifyPropertyChanged
    {
        public string Title { get; set; }
        public string Icon { get; set; }
        public string BackgroundColor { get; set; }
        public string Route { get; set; }
        private bool _isSelected;
        public bool IsSelected
        {
            get => _isSelected;
            set
            {
                if (_isSelected != value)
                {
                    _isSelected = value;
                    OnPropertyChanged(nameof(IsSelected));
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public class FlyoutMenuViewModel
    {
        public ObservableCollection<MenuItemViewModel> MenuItems { get; set; }

        public FlyoutMenuViewModel()
        {
            MenuItems = new ObservableCollection<MenuItemViewModel>
            {
                new MenuItemViewModel { Title = "Bangladeshian", Icon = "dotnet_bot", BackgroundColor = "DimGray"},
                new MenuItemViewModel { Title = "Management", Icon = "dotnet_bot", BackgroundColor = "DimGray"},
                new MenuItemViewModel { Title = "Profile", Icon = "dotnet_bot", BackgroundColor = "DimGray"},
            };
        }
        private void OnNavigated(object sender, ShellNavigatedEventArgs e)
    {
        var currentRoute = Current.CurrentState.Location.ToString();

        foreach (var item in MenuItems)
        {
            item.IsSelected = currentRoute.Contains(item.Route);
        }
    }
    }

    public Menu()
    {
        InitializeComponent();
        BindingContext = new FlyoutMenuViewModel();
    }
    
}