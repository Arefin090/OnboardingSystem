using OnboardingSystem.ViewModel;
using Microsoft.Maui.Controls;
using System.Collections.Generic;
using CommunityToolkit.Maui.Views;
using OnboardingSystem.Authentication;
using OnboardingSystem.Management.Components;
using OnboardingSystem.Enums;

namespace OnboardingSystem.Management;

public partial class ManagementPage : ContentPage
{
    private ManagementViewModel _viewModel;
    private String _route;
    private IAuthenticationService _authenticationService;
    public ManagementPage(IAuthenticationService authenticationService)
    {
        InitializeComponent();
        _authenticationService = authenticationService;
        

        // Programmatically modify UI or add labels dynamically if needed
        
    }
    protected override void OnNavigatedTo(NavigatedToEventArgs args)
    {
        base.OnNavigatedTo(args);
        _route = Shell.Current.CurrentState.Location.ToString().TrimStart('/');
        _viewModel = new ManagementViewModel(_route, _authenticationService);
        BindingContext = _viewModel;
        AddDataGrid(_route);
    }

    private void AddDataGrid(string route)
    {
        var table = MenuInitializer.menuItems.Find(table => table.TableName == route);
        var headers = table?.ColumnDefinitions.Select(c => c.Name).ToList();

        // Clear any existing column definitions
        HeaderGrid.ColumnDefinitions.Clear();
        var columnWidths = GetColumnDefinitions(headers.Count);
        foreach (var width in columnWidths) 
        {
            HeaderGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = width });
        }

        // Add labels to the grid
        int i = 0;
        for (i = 0; i < headers.Count; i++)
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
                    item.VerticalOptions = LayoutOptions.Start;
                    grid.Children.Add(item);
                    grid.SetColumn(item, column);
					column++;
                }
			}

            return grid;
        });

    }

    // private void InsertButton_Clicked(object sender, EventArgs e)
    // {
    //     this.ShowPopup(new DynamicUpdateForm(_authenticationService,_route, _viewModel, "Insert Form", CrudOperation.CREATE));
    // }
    //
    //
    //
    // private void FilterButton_Clicked(object sender, EventArgs e)
    // {
    //     this.ShowPopup(new DynamicUpdateForm(_authenticationService,_route, _viewModel, "Search Form", CrudOperation.READ));
    // }
    //
    // private void DeleteButton_Clicked(object? sender, EventArgs e)
    // {
    //     this.ShowPopup(new DynamicUpdateForm(_authenticationService,_route, _viewModel, "Delete Form", CrudOperation.DELETE));
    // }

    private void NextButton_Clicked(object? sender, EventArgs e)
    {
        _viewModel.NextPage();
    }
    private void PrevButton_Clicked(object? sender, EventArgs e)
    {
        _viewModel.PrevPage();
    }

    private void Picker_OnSelectedIndexChanged(object? sender, EventArgs e)
    {
        var operation = CrudOperation.READ;
        var title = "";
        if (_viewModel.SelectedOption == CrudOperation.READ.ToString())
        {
            operation = CrudOperation.READ;
            title = "Search Form";
        }else if (_viewModel.SelectedOption == CrudOperation.CREATE.ToString())
        {
            operation = CrudOperation.CREATE;
            title = "Create Form";
        }else if (_viewModel.SelectedOption == CrudOperation.DELETE.ToString())
        {
            operation = CrudOperation.DELETE;
            title = "Delete Form";
        }
        else
        {
            operation = CrudOperation.UPDATE;
            title = "Update Form";
        }
        this.ShowPopup(new DynamicUpdateForm(_authenticationService,_route, _viewModel, title,operation));
    }
}
