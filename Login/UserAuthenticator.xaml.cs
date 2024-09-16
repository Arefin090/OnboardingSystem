using Newtonsoft.Json;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace OnboardingSystem
{
    public class UserAuthenticator
    {
        // URL of the API endpoint for user login
        private static readonly string ApiUrl = "https://localhost:44339/api/User/login";

        public static async Task<(bool isValid, string errorMessage)> ValidateUserAsync(string email, string password)
        {
            // Check if email is provided
            if (string.IsNullOrEmpty(email))
            {
                return (false, "Email cannot be empty.");
            }

            // Validate email format
            if (!email.Contains('@'))
            {
                return (false, "Invalid email format.");
            }

            // Check if password is provided
            if (string.IsNullOrEmpty(password))
            {
                return (false, "Password cannot be empty.");
            }

            // Create the login request object
            var loginRequest = new
            {
                Username = email,
                Password = password
            };

            // Serialize the request object to JSON
            string jsonContent = JsonConvert.SerializeObject(loginRequest);
            var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

            // Create an HttpClient instance to send the request
            using (var httpClient = new HttpClient())
            {
                try
                {
                    // Send the POST request to the API
                    var response = await httpClient.PostAsync(ApiUrl, content);

                    // Check if the response is successful
                    if (response.IsSuccessStatusCode)
                    {
                        // Read and deserialize the response content
                        string jsonResponse = await response.Content.ReadAsStringAsync();
                        var tokenResponse = JsonConvert.DeserializeObject<TokenResponse>(jsonResponse);

                        // Store the authentication token securely
                        await SecureStorage.SetAsync("auth_token", tokenResponse.Token);

                        return (true, string.Empty);
                    }
                    else
                    {
                        // Handle specific HTTP error responses
                        if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                        {
                            return (false, "Invalid credentials. Please try again.");
                        }
                        else
                        {
                            string errorResponse = await response.Content.ReadAsStringAsync();
                            return (false, "Login failed: " + errorResponse);
                        }
                    }
                }
                catch (HttpRequestException ex)
                {
                    // Handle network errors
                    return (false, "Network error: " + ex.Message);
                }
                catch (Exception ex)
                {
                    // Handle unexpected errors
                    return (false, "Unexpected error: " + ex.Message);
                }
            }
        }
    }

    public class TokenResponse
    {
        public string Token { get; set; }
        public string RefreshToken { get; set; }
    }
}
