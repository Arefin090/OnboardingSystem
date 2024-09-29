using OnboardingSystem.ViewModel;
using Microsoft.Maui.Controls;
using System.Collections.Generic;
using CommunityToolkit.Maui.Views;
using OnboardingSystem.Management.Components;

namespace OnboardingSystem.Management;

public partial class ManagementPage : ContentPage
{
    private ManagementViewModel _viewModel;
    private String _route;

    public ManagementPage()
    {
        InitializeComponent();
        
        BindingContext = _viewModel;

        // Programmatically modify UI or add labels dynamically if needed
        
    }
    protected override void OnNavigatedTo(NavigatedToEventArgs args)
    {
        base.OnNavigatedTo(args);
        _route = Shell.Current.CurrentState.Location.ToString().TrimStart('/');
        _viewModel = new ManagementViewModel(_route);
        AddDataGrid(_route);
    }

    private void AddDataGrid(string route)
    {
        var table = MenuInitializer.menu.Find(table => table.TableName == route);
        var headers = table?.ColumnDefinitions.Select(c => c.Name).ToList();

        // Clear any existing column definitions
        HeaderGrid.ColumnDefinitions.Clear();
        var columnWidths = GetColumnDefinitions(headers.Count);
        foreach (var width in columnWidths) 
        {
            HeaderGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = width });
        }

        // Add labels to the grid
        for (int i = 0; i < headers.Count; i++)
        {
            var label = new Label
            {
                Text = headers[i],
                FontAttributes = FontAttributes.Bold,
                HorizontalOptions = LayoutOptions.Start,
                VerticalOptions = LayoutOptions.Center,
                FontSize = 18
            };
            HeaderGrid.Children.Add(label);
            HeaderGrid.SetColumn(label, i);
        }
        AddCollectionView(headers.Count());
    }

    private List<GridLength> GetColumnDefinitions(int numOfColumns)
    {
        var columnWidths = new List<GridLength>();
        for (int i = 0; i < numOfColumns; i++)
        {
            columnWidths.Add(new GridLength(1, GridUnitType.Star)); // Defines Column Size
        }
        return columnWidths;
    }

    private void AddCollectionView(int numOfColumns)
    {
        GridCollection.ItemsSource = _viewModel.Rows;
        var columnWidths = GetColumnDefinitions(numOfColumns);
        GridCollection.ItemTemplate = new DataTemplate(() =>
        {
            var grid = new Grid { Padding = 10 };
            foreach (var width in columnWidths)
            {
                grid.ColumnDefinitions.Add(new ColumnDefinition { Width = width });
            }
            foreach (var row in _viewModel.Rows)
            {   
                int column = 0;
                foreach (var key in row.Keys)
                {
                    var item = new Label();
                    item.SetBinding(Label.TextProperty, $"[{key}]");
                    item.Margin = new Thickness(2, 0, 2, 0);
                    grid.Children.Add(item);
                    grid.SetColumn(item, column);
                    column++;
                }
            }

            return grid;
        });

    }

    private void InsertButton_Clicked(object sender, EventArgs e)
    {
        this.ShowPopup(new DynamicUpdateForm(_route, _viewModel));
    }
}
