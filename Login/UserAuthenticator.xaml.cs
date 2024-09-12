namespace OnboardingSystem
{
    public class UserAuthenticator
    {
        private static readonly Dictionary<string, string> UserDatabase = new Dictionary<string, string> //temporary database
        {
            { "j@j.com", "123" },
            { "m@m.com", "456" }
        };

        public static (bool isValid, string errorMessage) ValidateUser(string email, string password) //validate email, password
        {
            if (string.IsNullOrEmpty(email))
            {
                return (false, "Email cannot be empty.");
            }

            if (!email.Contains('@'))
            {
                return (false, "Invalid email format.");
            }

            if (string.IsNullOrEmpty(password))
            {
                return (false, "Password cannot be empty.");
            }

            string lowercaseEmail = email.ToLowerInvariant();
            if (!UserDatabase.ContainsKey(lowercaseEmail))
            {
                return (false, "Email does not exist.");
            }

            string storedPassword = UserDatabase[lowercaseEmail];
            if (password != storedPassword)
            {
                return (false, "Incorrect password.");
            }

            return (true, string.Empty);
        }
    }
}