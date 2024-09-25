namespace OnboardingSystem
{
    public static class Constants
    {
        // API Base URL
        public const string API_BASE_URL = "https://localhost:44339";

        // API Endpoints
        public const string LOGIN_ENDPOINT = "/api/User/login";
        public const string REFRESH_TOKEN_ENDPOINT = "/api/User/refresh";
        public const string GET_USERS_ENDPOINT = "/api/User";
        public const string GET_USER_LIST_ENDPOINT = "/api/User/List";
        public const string REGISTER_ENDPOINT = "/api/User/register";

        // Local Storage Keys
        public const string ACCESS_TOKEN_KEY = "access_token";
        public const string REFRESH_TOKEN_KEY = "refresh_token";
        public const string TOKEN_EXPIRY_KEY = "token_expiry";

    

        // Roles
        public const string ROLE_ADMIN = "Admin";
        public const string ROLE_STAFF = "Staff";

        // Error Messages
        public const string GENERIC_ERROR = "An error occurred. Please try again.";
        public const string UNAUTHORIZED_ERROR = "You are not authorized to perform this action.";
        public const string NETWORK_ERROR = "Network error. Please check your internet connection.";

        // Success Messages
        public const string LOGIN_SUCCESS = "Login successful!";
        public const string REGISTER_SUCCESS = "Registration successful!";
    }
}