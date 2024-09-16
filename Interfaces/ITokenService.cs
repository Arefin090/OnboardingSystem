using System.Security.Claims;
using I72_Backend.Model;

namespace I72_Backend.Interfaces
{
    public interface ITokenService
    {
        string GenerateNewToken(User user);
        string GenerateRefreshToken();
        string EncodeRefreshToken(string refreshToken);  
        bool ValidateRefreshToken(string encodedToken, string providedToken);  

        ClaimsPrincipal GetPrincipalFromExpiredToken(string token);


    }

}
