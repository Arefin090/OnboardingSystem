using System.Net;
using CommunityToolkit.Maui.Markup;
using CommunityToolkit.Maui.Views;
using Microsoft.Maui.Controls.Compatibility.Platform.UWP;
using Microsoft.Maui.Controls.Shapes;
using OnboardingSystem.Enums;
using OnboardingSystem.ViewModel;
using System.Net.Http.Json;
using Newtonsoft.Json;
using OnboardingSystem.Authentication;
using OnboardingSystem.Models.ErrorResponse;
using OnboardingSystem.Models.SuccessResponse;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace OnboardingSystem.Management.Components;

public partial class DynamicUpdateForm : Popup
{
    private readonly String _tableName;
    private readonly ManagementViewModel _viewModel;
    private readonly DynamicFormViewModel _formViewModel = new DynamicFormViewModel();
    private readonly Dictionary<String, Entry> _entries;
    private readonly CrudOperation _crudOperation;
    private readonly IAuthenticationService _authenticationService;
    public DynamicUpdateForm(IAuthenticationService authenticationService,String tableName, ManagementViewModel viewModel, String title, CrudOperation operation)
	{
        _tableName = tableName;
        _viewModel = viewModel;
        _formViewModel.FormTitle = title;
        _authenticationService = authenticationService;
        _crudOperation = operation;
		InitializeComponent();
        BindingContext = _formViewModel;
        
        _entries = new Dictionary<String, Entry>();
        CreateDynamicEntries();
        if (_crudOperation == CrudOperation.READ)
        {
            RetrieveSavedEntryData();
        }
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

        if (_crudOperation == CrudOperation.READ)
        {
            Preferences.Set(_tableName, JsonSerializer.Serialize(requestData));
        }
        
        return requestData;
    }

    private void RetrieveSavedEntryData()
    {
        // Retrieve and deserialize from Preferences
        string? storedJson = Preferences.Get(_tableName, null);
        if(storedJson == null) return;
        var retrievedState = JsonSerializer.Deserialize<Dictionary<string, string>>(storedJson);
        foreach (var entry in _entries)
        {
            if(retrievedState.ContainsKey(entry.Key))
            {
                entry.Value.Text = retrievedState[entry.Key];
            }
        }
    }

    private async void Insert()
    {
        var requestData = RetrieveEntryData();
        HttpClient client = new HttpClient();
        client.BaseAddress = new Uri(Constants.API_BASE_URL);
        var token = await _authenticationService.GetValidTokenAsync();
        client.DefaultRequestHeaders.Add("Authorization", "Bearer " + token);

        // Read the response body as a string
        try
        {
            var response = await client.PostAsJsonAsync($"/api/Management?table={_tableName}", requestData);
            var responseBody = await response.Content.ReadAsStringAsync();
            var res = JsonSerializer.Deserialize<Dictionary<String, object>>(responseBody);
            switch (response.StatusCode)
            {
                case HttpStatusCode.OK:

                    Application.Current?.MainPage?.DisplayAlert("Insert Success", res?["message"].ToString(), "OK");
                    _viewModel.FetchData(new Dictionary<string, string>());
                    Close();
                    break;
                case HttpStatusCode.BadRequest:
                    Application.Current?.MainPage?.DisplayAlert("Insert Error", res?["message"].ToString(), "OK");
                    break;
            } 
        }
        catch (HttpRequestException ex)
        {
            // Handle specific network-related exceptions
            Console.WriteLine($"Network error: {ex.Message}");
            await Application.Current.MainPage.DisplayAlert("Error", ex.Message, "OK");
        }
        catch (Exception ex)
        {
            Application.Current.MainPage.DisplayAlert("Insert Error", ex.Message, "OK");
        }
        finally
        {
            _viewModel.ResetPage();
        }

    }

    private async void Delete()
    {
        var requestData = RetrieveEntryData();
        HttpClient client = new HttpClient();
        client.BaseAddress = new Uri(Constants.API_BASE_URL);

        // Create the DELETE request with a body
        // Retrieve the token securely
        var token = await _authenticationService.GetValidTokenAsync();
        var request = new HttpRequestMessage
        {
            Method = HttpMethod.Delete,
            Headers =
            {
                { "x-ms-version", "2013-06-01" },
                { "Authorization", $"Bearer {token}" }
            },
            RequestUri = new Uri($"/api/Management/batch?table={_tableName}", UriKind.Relative),
            Content = JsonContent.Create(requestData) // Set the JSON content here
        };
        try
        {
            // Perform DELETE request
            var response = await client.SendAsync(request);
            // Read the response body as a string
            var responseBody = await response.Content.ReadAsStringAsync();
            switch (response.StatusCode)
            {
                case HttpStatusCode.OK:
                    var res = JsonSerializer.Deserialize<Dictionary<String, object>>(responseBody);
                    Application.Current?.MainPage?.DisplayAlert("Delete Success", res?["message"].ToString(), "OK");
                    _viewModel.FetchData(new Dictionary<string, string>());
                    Close();
                    break;
                case HttpStatusCode.BadRequest:
                    Application.Current?.MainPage?.DisplayAlert("Delete Error", "At least one fields should be filled", "OK");
                    break;
            };
        }
        catch (HttpRequestException ex)
        {
            // Handle specific network-related exceptions
            Console.WriteLine($"Network error: {ex.Message}");
            await Application.Current.MainPage.DisplayAlert("Error", ex.Message, "OK");
        }
        catch (Exception ex)
        {
            Application.Current.MainPage.DisplayAlert("Insert Error", ex.Message, "OK");
        }
        finally
        {
            _viewModel.ResetPage();
        }
    }

    private void OnSubmitClicked(object sender, EventArgs e)
    {
        switch (_crudOperation)
        {
            case CrudOperation.CREATE:
                Insert();
                break;
            case CrudOperation.DELETE:
                Delete();
                break;
            case CrudOperation.READ:
                _viewModel.Page = 1;
                _viewModel.FetchData(RetrieveEntryData());
                Close();
                break;
        }
    }

    private void OnCancelClicked(object sender, EventArgs e)
    {
        // Close the popup
        Close();
    }
}