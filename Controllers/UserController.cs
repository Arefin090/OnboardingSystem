using I72_Backend.Interfaces;
using I72_Backend.Model;
using I72_Backend.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace I72_Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserRepository _userRepository;
        private readonly ITokenService _tokenService;

        // Constructor to inject dependencies
        public UserController(IUserRepository userRepository, ITokenService tokenService)
        {
            _userRepository = userRepository;
            _tokenService = tokenService;
        }
        

        // GET: api/User
        // This endpoint is accessible to both Admin and Staff roles.
        [Authorize(Roles = "Admin,Staff")]
        [HttpGet]
        public ActionResult<IEnumerable<User>> GetUsers()
        {
            // Fetch the list of users from the repository
            var users = _userRepository.GetUsers();
            return Ok(users);
        }

        // GET: api/User/{username}
        // This endpoint is accessible to Admin only.
        [Authorize(Roles = "Admin")]
        [HttpGet("{username}")]
        public ActionResult<User> GetUserByUsername(string username)
        {
            // Fetch a user by username from the repository
            var user = _userRepository.GetUserByUsername(username?.Trim().ToLower());
            if (user == null)
            {
                return NotFound();
            }
            return Ok(user);
        }

        // POST: api/User/login
        // This endpoint is accessible to everyone (AllowAnonymous).
        [AllowAnonymous]
        [HttpPost("login")]
        public ActionResult Login([FromBody] LoginRequest loginRequest)
        {
            // Verify user credentials
            var user = _userRepository.GetUserByUsername(loginRequest.Username?.Trim().ToLower());
            if (user == null || !_userRepository.VerifyPassword(loginRequest.Password, user.Password))
            {
                return Unauthorized("Invalid credentials");
            }

            // Generate a new JWT token and refresh token
            var tokenString = _tokenService.GenerateNewToken(user);
            var refreshToken = _tokenService.GenerateRefreshToken();
            var encodedRefreshToken = _tokenService.EncodeRefreshToken(refreshToken);

            // Store the refresh token for the user
            _userRepository.SetUserRefreshToken(user.Username, encodedRefreshToken);

            return Ok(new
            {
                Token = tokenString,
                RefreshToken = refreshToken
            });
        }

        // POST: api/User/refresh
        // This endpoint is accessible to everyone (AllowAnonymous).
        [AllowAnonymous]
        [HttpPost("refresh")]
        public ActionResult Refresh([FromBody] TokenRefreshRequest request)
        {
            // Validate the provided refresh token and generate a new token
            var principal = _tokenService.GetPrincipalFromExpiredToken(request.Token);
            var username = principal.Identity.Name;

            var user = _userRepository.GetUserByUsername(username);
            if (user == null || !_tokenService.ValidateRefreshToken(user.RefreshToken, request.RefreshToken))
            {
                return Unauthorized("Invalid refresh token");
            }

            var newToken = _tokenService.GenerateNewToken(user);
            var newRefreshToken = _tokenService.GenerateRefreshToken();
            var encodedNewRefreshToken = _tokenService.EncodeRefreshToken(newRefreshToken);

            // Update the user's refresh token
            _userRepository.SetUserRefreshToken(username, encodedNewRefreshToken);

            return Ok(new
            {
                Token = newToken,
                RefreshToken = newRefreshToken  // Send the new refresh token back to the client
            });
        }

        // POST: api/User/register
        // This endpoint is accessible to Admin only.
        [Authorize(Roles = "Admin")]
        [HttpPost("register")]
        public ActionResult Register([FromBody] User user)
        {
            // Normalize the username
            user.Username = user.Username?.Trim().ToLower();

            // Check if the username already exists
            var existingUser = _userRepository.GetUserByUsername(user.Username);
            if (existingUser != null)
            {
                return Conflict("Username already exists");
            }

            // Hash the user's password
            user.Password = BCrypt.Net.BCrypt.HashPassword(user.Password);

            // Assign a default role if none is provided
            if (string.IsNullOrWhiteSpace(user.Role))
            {
                user.Role = "Staff"; // Default to "Staff"
            }

            // Add the new user to the repository
            _userRepository.AddUser(user);

            return Ok(new { Message = "User registered successfully" });
        }

        // DELETE: api/User/{id}
        // This endpoint is accessible to Admin only.
        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public ActionResult DeleteUserById(int id)
        {
            // Fetch the user by ID
            var user = _userRepository.GetUserById(id);
            if (user == null)
            {
                return NotFound($"Failed to delete the user with ID {id}. User not found.");
            }

            try
            {
                // Delete the user from the repository
                _userRepository.DeleteUser(user);
                return Ok($"User with ID {id} deleted successfully.");
            }
            catch (Exception ex)
            {
                // Handle any errors that occur during deletion
                return StatusCode(StatusCodes.Status500InternalServerError, $"Failed to delete the user with ID {id}. Error: {ex.Message}");
            }
        }
    }
}
