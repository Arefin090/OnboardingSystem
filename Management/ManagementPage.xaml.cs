using OnboardingSystem.ViewModel;
using Microsoft.Maui.Controls;
using System.Collections.Generic;

namespace OnboardingSystem.Management;

public partial class ManagementPage : ContentPage
{
    private ManagementViewModel _viewModel;

    public ManagementPage()
    {
        InitializeComponent();
        _viewModel = new ManagementViewModel();
        BindingContext = _viewModel;

        // Programmatically modify UI or add labels dynamically if needed
        AddDataGrid();
    }

    private async void AddDataGrid()
    {
        var headers = new List<string> { "Staff ID", "Name", "Role", "Phone", "Address", "Branch" };

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
                VerticalOptions = LayoutOptions.Center
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
        GridCollection.ItemsSource = _viewModel.StaffMembers;
        var columnWidths = GetColumnDefinitions(numOfColumns);
        GridCollection.ItemTemplate = new DataTemplate(() =>
        {
            var grid = new Grid { Padding = 10 };
            foreach (var width in columnWidths)
            {
                grid.ColumnDefinitions.Add(new ColumnDefinition { Width = width });
            }
            foreach (var row in _viewModel.StaffMembers)
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
}
