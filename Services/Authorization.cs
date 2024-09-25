using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using OnboardingSystem.Authentication;
using OnboardingSystem.Models;

namespace OnboardingSystem.Helpers
{
    public class Authorization
    {
        private readonly IAuthenticationService _authService;

        public Authorization(IAuthenticationService authService)
        {
            _authService = authService;
        }

        public async Task<bool> IsUserAuthorized(params string[] allowedRoles)
        {
            bool isAuthenticated = await _authService.IsAuthenticatedAsync();
            if (!isAuthenticated)
            {
                await Application.Current.MainPage.DisplayAlert("Unauthorized", "You are not authenticated. Please log in.", "OK");
                return false;
            }

            var token = await _authService.GetTokenAsync();
            string userRole = GetUserRoleFromToken(token);
            
            if (!allowedRoles.Contains(userRole))
            {
                await Application.Current.MainPage.DisplayAlert("Unauthorized", "You do not have permission to access this data.", "OK");
                return false;
            }

            return true;
        }

        public string GetUserRoleFromToken(string token)
        {
            var handler = new JwtSecurityTokenHandler();
            var jwtToken = handler.ReadJwtToken(token);

            var roleClaim = jwtToken.Claims.FirstOrDefault(c => c.Type == "role" || c.Type == ClaimTypes.Role)?.Value;
            return string.IsNullOrEmpty(roleClaim) ? "Unknown" : roleClaim;
        }

        public bool IsTokenExpired(string token)
        {
            var handler = new JwtSecurityTokenHandler();
            var jwtToken = handler.ReadJwtToken(token);
            return DateTime.UtcNow >= jwtToken.ValidTo.AddMinutes(-5);
        }

        public async Task<string> GetValidTokenAsync()
        {
            var token = await _authService.GetTokenAsync();
            if (IsTokenExpired(token))
            {
                var refreshToken = await SecureStorage.GetAsync(Constants.REFRESH_TOKEN_KEY);
                if (string.IsNullOrEmpty(refreshToken))
                {
                    return null;
                }

                var (success, newAccessToken, newRefreshToken) = await _authService.RefreshTokenAsync(refreshToken);
                if (success)
                {
                    return newAccessToken;
                }
            }
            return token;
        }
    }
}