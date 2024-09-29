using CommunityToolkit.Maui.Markup;
using CommunityToolkit.Maui.Views;
using Microsoft.Maui.Controls.Compatibility.Platform.UWP;
using Microsoft.Maui.Controls.Shapes;
using OnboardingSystem.ViewModel;
using System.Net.Http.Json;

namespace OnboardingSystem.Management.Components;

public partial class DynamicUpdateForm : Popup
{
    private String _tableName;
    private ManagementViewModel _viewModel;
    private Dictionary<String, Entry> _entries= new Dictionary<String, Entry>();
    public DynamicUpdateForm(String tableName, ManagementViewModel viewModel)
	{
        _tableName = tableName;
        _viewModel = viewModel;
		InitializeComponent();
        CreateDynamicEntries();
	}
    private void CreateDynamicEntries()
    {
        // Example entries to create
        var tableSchema = MenuInitializer.menu.Find(x => x.TableName == _tableName);
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
                BackgroundColor = Color.FromHex("#555555"),
                TextColor = Colors.Black,
                PlaceholderColor = Color.FromHex("#aaaaaa"),
                Margin = new Thickness(0, 5),
                HeightRequest = 14
            };

            _entries.Add(entry.Key, entryField);

            // Create a Border for Entry
            var border = new Border
            {
                StrokeShape = new RoundRectangle { CornerRadius = 40 },
                Content = entryField
            };
            // Add to the StackLayout
            DynamicEntryStack.Children.Add(label);
            DynamicEntryStack.Children.Add(border);
        }
    }

    private async void OnSubmitClicked(object sender, EventArgs e)
    {
        var requestData = new Dictionary<string, string?>();
        // Retrieve the text values from the entries
        foreach (var entry in _entries)
        {
            string? textValue = string.IsNullOrWhiteSpace(entry.Value.Text) ? null : entry.Value.Text;
            requestData.Add(entry.Key, textValue);
        }
        HttpClient client = new HttpClient();
        client.BaseAddress = new Uri(Constants.API_BASE_URL);

        var response = await client.PostAsJsonAsync($"/api/Management?table={_tableName}", requestData);
        try
        {
            response.EnsureSuccessStatusCode();
            _viewModel.FetchData();
            Close();
        }
        catch (HttpRequestException ex)
        {
            Console.WriteLine(ex.Message);
            Console.WriteLine(response.Content.ReadAsStringAsync());
        }
    }

    private void OnCancelClicked(object sender, EventArgs e)
    {
        // Close the popup
        Close();
    }
}