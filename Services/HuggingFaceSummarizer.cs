using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

public class HuggingFaceSummarizer
{
    private readonly HttpClient _httpClient;

    public HuggingFaceSummarizer()
    {
        _httpClient = new HttpClient();
        _httpClient.BaseAddress = new Uri("https://api-inference.huggingface.co/models/");
        _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", "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");
    }

   public async Task<string> SummarizeAsync(string inputText)
{
    if (string.IsNullOrEmpty(inputText))
    {
        return "Input text is empty. Please provide valid content.";
    }

    var requestBody = new { inputs = inputText };

    // Serialize the request body to JSON
    var jsonContent = new StringContent(JsonSerializer.Serialize(requestBody), System.Text.Encoding.UTF8, "application/json");

    try
    {
        // Call the Hugging Face API
        var response = await _httpClient.PostAsync("facebook/bart-large-cnn", jsonContent);

        if (response.IsSuccessStatusCode)
        {
            var responseContent = await response.Content.ReadAsStringAsync();
            var jsonDocument = JsonDocument.Parse(responseContent);

            // Check if the response is an array or object
            string summaryText;
            if (jsonDocument.RootElement.ValueKind == JsonValueKind.Array)
            {
                // If it's an array, get the first element
                summaryText = jsonDocument.RootElement[0].GetProperty("summary_text").GetString();
            }
            else if (jsonDocument.RootElement.ValueKind == JsonValueKind.Object)
            {
                // If it's an object, get the summary text
                summaryText = jsonDocument.RootElement.GetProperty("summary_text").GetString();
            }
            else
            {
                return "Unexpected response format from API.";
            }

            // Clean up the summary: remove redundant characters or format issues
            summaryText = CleanUpSummary(summaryText);

            return summaryText;
        }
        else
        {
            // Extract the detailed error message from the response
            var errorDetails = await response.Content.ReadAsStringAsync();
            return $"Failed to call Hugging Face API: {response.StatusCode}, Details: {errorDetails}";
        }
    }
    catch (Exception ex)
    {
        return $"Error summarizing text: {ex.Message}";
    }
}

private string CleanUpSummary(string summary)
{
    // Perform post-processing cleanup on the summary to remove repetitive or garbled text
    // For instance, removing odd capitalizations or redundant terms
    return summary.Replace("AUTo_INCrement", "AUTO_INCREMENT")
                  .Replace("VArchitecture", "VARCHAR")
                  .Replace("INTAUTO_", "INT AUTO_")
                  .Replace("tableSales", "Table Sales")
                  .Trim(); // Trim any unnecessary spaces
}

}

