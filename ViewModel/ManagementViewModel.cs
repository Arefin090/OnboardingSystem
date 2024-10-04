using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Net;
using System.Net.Http.Json;
using System.Runtime.CompilerServices;
using System.Text.Json;
using OnboardingSystem.Models;

namespace OnboardingSystem.ViewModel;

public class ManagementViewModel : INotifyPropertyChanged
{
    public ObservableCollection<Dictionary<String, String>> Rows { get; set; }
    public event PropertyChangedEventHandler? PropertyChanged;
    private String _tableName = String.Empty;
    private int _page;
    public int Page
    {
        get => _page;
        set
        {
            _page = value;
            OnPropertyChanged(nameof(Page));
        }
    }
    private int TotalPage { get; set; }
    private bool _isLoading;
    public bool IsLoading
    {
        get => _isLoading;
        set
        {
            _isLoading = value;
            OnPropertyChanged(nameof(IsLoading)); // Notify the view of property change
        }
    }

    public ManagementViewModel(String tableName)
    {
        _tableName = tableName;
        _page = 1;
        // Populate the collection with initial data
        Rows = new ObservableCollection<Dictionary<string, string>>();
        FetchData(new Dictionary<string, string>());
    }
    
    public async void FetchData(Dictionary<string, string> requestData)
    {
        HttpClient client = new HttpClient();
        client.BaseAddress = new Uri(Constants.API_BASE_URL);
        try
        {
            // Perform the POST request
            IsLoading = true;
            var response =
                await client.PostAsJsonAsync($"/api/Management/get-data?table={_tableName}&Page={Page - 1}&PageSize=10",
                    requestData);

            // Ensure the response was successful
            response.EnsureSuccessStatusCode();
            // Read the response content
            var resultString = await response.Content.ReadAsStringAsync();
            Rows.Clear();

            // Deserialize to a dictionary
            var result = JsonSerializer.Deserialize<Dictionary<string, object>>(resultString);
            var table = MenuInitializer.GetItemByTableName(_tableName);
            if (result != null && result.TryGetValue("data", out var dataObj))
            {
                var dataArray = JsonSerializer.Deserialize<List<Dictionary<string, object>>>(dataObj.ToString());

                foreach (var item in dataArray)
                {
                    var stringDict = new Dictionary<string, string>();

                    foreach (var col in table.ColumnDefinitions)
                    {
                        stringDict[col.Name] = item[col.Name].ToString(); // Convert value to string
                    }

                    Rows.Add(stringDict);
                }
            }
            else
            {
                Console.WriteLine("No 'data' found in the response.");
            }

            if (result != null && result.TryGetValue("totalPage", out var totalPage))
            {
                TotalPage = Convert.ToInt32(totalPage.ToString());
            }
        }
        catch (HttpRequestException ex)
        {
            Console.WriteLine($"Request error: {ex.Message}");
        }
        finally
        {
            IsLoading = false;
        }
    }

    public void NextPage()
    {
        if (Page == TotalPage - 1) return;
        Page += 1;
        // Retrieve and deserialize from Preferences
        string? storedJson = Preferences.Get(_tableName, null);
        Dictionary<String, String> body = new Dictionary<String, String>();
        if (storedJson != null)
        {
            var retrievedState = JsonSerializer.Deserialize<Dictionary<string, string>>(storedJson);
            body = retrievedState;
        }
        FetchData(body);
    }

    public void PrevPage()
    {
        if(Page == 0) return;
        Page -= 1;
        // Retrieve and deserialize from Preferences
        string? storedJson = Preferences.Get(_tableName, null);
        Dictionary<String, String> body = new Dictionary<String, String>();
        if (storedJson != null)
        {
            var retrievedState = JsonSerializer.Deserialize<Dictionary<string, string>>(storedJson);
            body = retrievedState;
        }
        FetchData(body);
    }
    protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}