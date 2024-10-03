using CommunityToolkit.Maui.Markup;
using CommunityToolkit.Maui.Views;
using Microsoft.Maui.Controls.Compatibility.Platform.UWP;
using Microsoft.Maui.Controls.Shapes;
using OnboardingSystem.Enums;
using OnboardingSystem.ViewModel;
using System.Net.Http.Json;

namespace OnboardingSystem.Management.Components;

public partial class DynamicUpdateForm : Popup
{
    private String _tableName;
    private ManagementViewModel _viewModel;
    private DynamicFormViewModel _formViewModel = new DynamicFormViewModel();
    private Dictionary<String, Entry> _entries= new Dictionary<String, Entry>();
    private CrudOperation _crudOperation;
    public DynamicUpdateForm(String tableName, ManagementViewModel viewModel, String title, CrudOperation operation)
	{
        _tableName = tableName;
        _viewModel = viewModel;
        _formViewModel.FormTitle = title;
        _crudOperation = operation;
		InitializeComponent();
        BindingContext = _formViewModel;
        CreateDynamicEntries();
	}
    private void CreateDynamicEntries()
    {
        // Example entries to create
        var tableSchema = MenuInitializer.menuItems.Find(x => x.TableName == _tableName);
        var entryData = tableSchema.ColumnDefinitions.Select(col => new { Label = col.Name, Placeholder = $"Enter {col.Name}", Keyboard = Keyboard.Default, Key = col.Name }).ToList();

        foreach (var entry in entryData)
        {
            // Create and configure the label
            var label = new Label
            {
                Text = entry.Label,
                TextColor = Colors.Black,
                Margin = new Thickness(0, 10, 0, 0)
            };

            // Create the Entry
            var entryField = new Entry
            {
                Placeholder = entry.Placeholder,
                Keyboard = entry.Keyboard,
                TextColor = Colors.White,
                BackgroundColor = Color.FromArgb("#303030"),
                Margin = new Thickness(0, 5),
            };

            _entries.Add(entry.Key, entryField);

            // Create a Border for Entry
            var border = new Border
            {
                StrokeShape = new RoundRectangle { CornerRadius = 10 },
                Content = entryField,
                // HeightRequest = entryField.HeightRequest,
            };
            // Add to the StackLayout
            DynamicEntryStack.Children.Add(label);
            DynamicEntryStack.Children.Add(entryField);
        }
    }
    private Dictionary<string, string?> RetrieveEntryData()
    {
        var requestData = new Dictionary<string, string?>();
        // Retrieve the text values from the entries
        foreach (var entry in _entries)
        {
            if(string.IsNullOrWhiteSpace(entry.Value.Text))
            {
                continue;
            }
            requestData.Add(entry.Key, entry.Value.Text);
        }
        return requestData;
    }

    private async void Insert()
    {
        var requestData = RetrieveEntryData();
        HttpClient client = new HttpClient();
        client.BaseAddress = new Uri(Constants.API_BASE_URL);

        var response = await client.PostAsJsonAsync($"/api/Management?table={_tableName}", requestData);
        try
        {
            response.EnsureSuccessStatusCode();
            _viewModel.FetchData(new Dictionary<string, string>());
            Close();
        }
        catch (HttpRequestException ex)
        {
            Console.WriteLine(ex.Message);
            Console.WriteLine(response.Content.ReadAsStringAsync());
        }
    }

    private void OnSubmitClicked(object sender, EventArgs e)
    {
        switch (_crudOperation)
        {
            case CrudOperation.CREATE:
                Insert();
                break;
            case CrudOperation.READ:
                _viewModel.FetchData(RetrieveEntryData());
                break;
            default:
                break;
        }
        Close();
    }

    private void OnCancelClicked(object sender, EventArgs e)
    {
        // Close the popup
        Close();
    }
}