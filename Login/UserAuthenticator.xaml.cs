using Newtonsoft.Json;
using OnboardingSystem.Models;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace OnboardingSystem.Authentication
{
    public interface IAuthenticationService
    {
        Task<(bool isValid, string errorMessage)> ValidateUserAsync(string email, string password);
        Task<bool> IsAuthenticatedAsync();
        Task<string> GetTokenAsync();
        Task<string> GetValidTokenAsync();
        Task ClearAuthStateAsync();
    }

    public class AuthenticationService : IAuthenticationService
    {
        private readonly HttpClient _httpClient;

        public AuthenticationService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<(bool isValid, string errorMessage)> ValidateUserAsync(string email, string password)
        {
            if (string.IsNullOrEmpty(email) || !email.Contains('@'))
                return (false, "Invalid email format.");

            if (string.IsNullOrEmpty(password))
                return (false, "Password cannot be empty.");

            var loginRequest = new { Username = email, Password = password };
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

        public async Task<string> GetValidTokenAsync()
        {
            var token = await SecureStorage.GetAsync(Constants.ACCESS_TOKEN_KEY);
            if (string.IsNullOrEmpty(token) || IsTokenExpired(token))
            {
                var refreshToken = await SecureStorage.GetAsync(Constants.REFRESH_TOKEN_KEY);
                if (string.IsNullOrEmpty(refreshToken))
                {
                    return null;
                }

                var (success, newAccessToken, newRefreshToken) = await RefreshTokenAsync(refreshToken);
                if (success)
                {
                    await SecureStorage.SetAsync(Constants.ACCESS_TOKEN_KEY, newAccessToken);
                    await SecureStorage.SetAsync(Constants.REFRESH_TOKEN_KEY, newRefreshToken);
                    return newAccessToken;
                }
                return null;
            }
            return token;
        }

        public async Task ClearAuthStateAsync()
        {
            try
            {
                SecureStorage.Remove(Constants.ACCESS_TOKEN_KEY);
                SecureStorage.Remove(Constants.REFRESH_TOKEN_KEY);
            }
            catch (Exception ex)
            {
                // Log the exception if you have a logging mechanism
                Console.WriteLine($"Error clearing auth state: {ex.Message}");
            }
        }

        private async Task<(bool success, string accessToken, string refreshToken)> RefreshTokenAsync(string refreshToken)
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
                    return (true, tokenResponse.Token, tokenResponse.RefreshToken);
                }

                return (false, null, null);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error refreshing token: {ex.Message}");
                return (false, null, null);
            }
        }

        private bool IsTokenExpired(string token)
        {
            var handler = new JwtSecurityTokenHandler();
            var jwtToken = handler.ReadJwtToken(token);
            return DateTime.UtcNow >= jwtToken.ValidTo;
        }

        public async Task<string> GetUserRoleAsync()
        {
            var token = await GetValidTokenAsync();
            if (string.IsNullOrEmpty(token))
                return null;

            var handler = new JwtSecurityTokenHandler();
            var jwtToken = handler.ReadJwtToken(token);

            var roleClaim = jwtToken.Claims.FirstOrDefault(claim => claim.Type == "role" || claim.Type == ClaimTypes.Role);
            return roleClaim?.Value;
        }

    }
}