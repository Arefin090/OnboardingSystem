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
    public ManagementViewModel(String tableName)
    {
        _tableName = tableName;
        // Populate the collection with initial data
        Rows = new ObservableCollection<Dictionary<string, string>>();
        FetchData();
    }

    public async void FetchData()
    {
        HttpClient client = new HttpClient();
        client.BaseAddress = new Uri(Constants.API_BASE_URL);

        try
        {
            // Perform the POST request
            var response = await client.PostAsJsonAsync($"/api/Management/get-data?table={_tableName}&Page={0}&PageSize=10", new Dictionary<string, string>());

            // Ensure the response was successful
            response.EnsureSuccessStatusCode();

            // Read the response content
            var resultString = await response.Content.ReadAsStringAsync();
            Rows.Clear();

            // Deserialize to a dictionary
            var result = JsonSerializer.Deserialize<Dictionary<string, object>>(resultString);
            
            // Access the "value" key
            if (result != null && result.TryGetValue("value", out var valueObj))
            {
                var valueDict = JsonSerializer.Deserialize<Dictionary<string, object>>(valueObj.ToString());

                // Access the "data" key
                if (valueDict != null && valueDict.TryGetValue("data", out var dataObj))
                {
                    var dataArray = JsonSerializer.Deserialize<List<Dictionary<string, object>>>(dataObj.ToString());

                    foreach (var item in dataArray)
                    {
                        var stringDict = new Dictionary<string, string>();

                        foreach (var kvp in item)
                        {
                            stringDict[kvp.Key] = kvp.Value.ToString(); // Convert value to string
                        }

                        Rows.Add(stringDict);
                    }
                }
                else
                {
                    Console.WriteLine("No 'data' found in the response.");
                }
            }
            else
            {
                Console.WriteLine("No 'value' found in the response.");
            }
        }
        catch (HttpRequestException ex)
        {
            Console.WriteLine($"Request error: {ex.Message}");
        }
    }


    protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}