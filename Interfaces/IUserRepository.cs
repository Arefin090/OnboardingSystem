using I72_Backend.Model;
using System.Collections.Generic;

namespace I72_Backend.Interfaces
{
    public interface IUserRepository
    {
        // Get a collection of all users
        ICollection<User> GetUsers();

        // Get a single user by username
        User GetUserByUsername(string username);

        // Verify if a password matches the stored password hash
        bool VerifyPassword(string plainTextPassword, string hashedPassword);

        // Login method that returns a boolean indicating success or failure
        bool Login(string username, string plainTextPassword);

        // Add a new user to the database
        void AddUser(User user);

        User GetUserById(int id);
        void DeleteUser(User user);

        void UpdateUser(User user); 
        void SetUserRefreshToken(string username, string refreshToken); 

    }
}
