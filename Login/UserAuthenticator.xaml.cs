using Newtonsoft.Json;
using OnboardingSystem.Models;
using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;


namespace OnboardingSystem.Authentication
{
    public interface IAuthenticationService
    {
        Task<(bool isValid, string errorMessage)> ValidateUserAsync(string username, string password);
        Task<bool> IsAuthenticatedAsync();
        Task<string> GetTokenAsync();
        Task<(bool Success, string AccessToken, string RefreshToken)> RefreshTokenAsync(string refreshToken);
    }

    public interface IUserService
    {
        Task<string> GetUserListAsync();
    }

    public class AuthenticationService : IAuthenticationService
    {
        private readonly HttpClient _httpClient;

        public AuthenticationService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<(bool isValid, string errorMessage)> ValidateUserAsync(string username, string password)
        {
            if (string.IsNullOrEmpty(username))
                return (false, "Username cannot be empty");

            if (string.IsNullOrEmpty(password))
                return (false, "Password cannot be empty.");

            var loginRequest = new { Username = username, Password = password };
            var content = new StringContent(JsonConvert.SerializeObject(loginRequest), Encoding.UTF8, "application/json");

            try
            {
                var response = await _httpClient.PostAsync($"{Constants.API_BASE_URL}{Constants.LOGIN_ENDPOINT}", content);

                if (response.IsSuccessStatusCode)
                {
                    var jsonResponse = await response.Content.ReadAsStringAsync();
                    var tokenResponse = JsonConvert.DeserializeObject<TokenResponse>(jsonResponse);

                    await SecureStorage.SetAsync(Constants.ACCESS_TOKEN_KEY, tokenResponse.Token);
                    await SecureStorage.SetAsync(Constants.REFRESH_TOKEN_KEY, tokenResponse.RefreshToken);

                    return (true, Constants.LOGIN_SUCCESS);
                }
                
                if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                    return (false, Constants.UNAUTHORIZED_ERROR);

                var errorResponse = await response.Content.ReadAsStringAsync();
                return (false, $"Login failed: {errorResponse}");
            }
            catch (HttpRequestException)
            {
                return (false, Constants.NETWORK_ERROR);
            }
            catch (Exception)
            {
                return (false, Constants.GENERIC_ERROR);
            }
        }

        public async Task<bool> IsAuthenticatedAsync()
        {
            var token = await SecureStorage.GetAsync(Constants.ACCESS_TOKEN_KEY);
            return !string.IsNullOrEmpty(token);
        }

        public async Task<string> GetTokenAsync()
        {
            return await SecureStorage.GetAsync(Constants.ACCESS_TOKEN_KEY);
        }
         public async Task<(bool Success, string AccessToken, string RefreshToken)> RefreshTokenAsync(string refreshToken)
        {
            var refreshRequest = new { RefreshToken = refreshToken };
            var content = new StringContent(JsonConvert.SerializeObject(refreshRequest), Encoding.UTF8, "application/json");

            try
            {
                var response = await _httpClient.PostAsync($"{Constants.API_BASE_URL}{Constants.REFRESH_TOKEN_ENDPOINT}", content);

                if (response.IsSuccessStatusCode)
                {
                    var jsonResponse = await response.Content.ReadAsStringAsync();
                    var tokenResponse = JsonConvert.DeserializeObject<TokenResponse>(jsonResponse);

                    await SecureStorage.SetAsync(Constants.ACCESS_TOKEN_KEY, tokenResponse.Token);
                    await SecureStorage.SetAsync(Constants.REFRESH_TOKEN_KEY, tokenResponse.RefreshToken);

                    return (true, tokenResponse.Token, tokenResponse.RefreshToken);
                }

                return (false, null, null);
            }
            catch (Exception ex)
            {
                // Log the exception if you have a logging mechanism
                Console.WriteLine($"Error refreshing token: {ex.Message}");
                return (false, null, null);
            }
        }
    }

    public class UserService : IUserService
    {
        private readonly HttpClient _httpClient;
        private readonly IAuthenticationService _authService;

        public UserService(HttpClient httpClient, IAuthenticationService authService)
        {
            _httpClient = httpClient;
            _authService = authService;
        }

        public async Task<string> GetUserListAsync()
        {
            var token = await _authService.GetTokenAsync();
            _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            var response = await _httpClient.GetAsync($"{Constants.API_BASE_URL}{Constants.GET_USER_LIST_ENDPOINT}");
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadAsStringAsync();
            }
            
            // Handle errors as necessary
            return null;
        }
    }

}