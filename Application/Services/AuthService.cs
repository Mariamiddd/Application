using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Application.InterfaceServices;
using Core.Interfaces;
using Core.Models;
using Core.Enums;

namespace Application.Services
{
    // AuthService class implements IAuthService and provides methods for user registration, login, and password verification.
    public class AuthService : IAuthService
    {
        // readonly field for file manager to handle user data storage and retrieval
        private readonly IFileManager _fileManager;

        // constructor with dependency injection for file manager
        public AuthService(IFileManager fileManager)
        {
            // throw an exception if fileManager is null to ensure proper initialization
            _fileManager = fileManager ?? throw new ArgumentNullException(nameof(fileManager));
        }

        // Register a new user asynchronously with email, password, first name, last name
        public async Task<User> RegisterAsync(string email, string password, string firstName, string lastName, Roles role = Roles.User)
        {
            ValidateRegistrationInput(email, password, firstName, lastName);

            if (await _fileManager.GetUserByEmailAsync(email) != null)
                throw new InvalidOperationException("User with this email already exists.");

            var hashedPassword = BCrypt.Net.BCrypt.HashPassword(password);
            var userId = await GenerateNewUserIdAsync();
            var user = CreateUser(userId, email, hashedPassword, firstName, lastName, role);

            await _fileManager.AddUserAsync(user);
            return user;
        }

        // Login a user asynchronously with email and password
        public async Task<User?> LoginAsync(string email, string password)
        {
            ValidateCredentials(email, password);

            var user = await _fileManager.GetUserByEmailAsync(email);
            if (user == null) return null;

            return BCrypt.Net.BCrypt.Verify(password, user.Password) ? user : null;
        }

        // Verify a user's password asynchronously with email and password
        public async Task<bool> VerifyPasswordAsync(string email, string password)
        {
            ValidateCredentials(email, password);

            var user = await _fileManager.GetUserByEmailAsync(email);
            return user != null && BCrypt.Net.BCrypt.Verify(password, user.Password);
        }

        // Validate registration input fields for email, password, first name, and last name
        private void ValidateRegistrationInput(string email, string password, string firstName, string lastName)
        {
            if (string.IsNullOrWhiteSpace(email))
                throw new ArgumentException("Email cannot be empty.");
            if (string.IsNullOrWhiteSpace(password))
                throw new ArgumentException("Password cannot be empty.");
            if (string.IsNullOrWhiteSpace(firstName))
                throw new ArgumentException("First name cannot be empty.");
            if (string.IsNullOrWhiteSpace(lastName))
                throw new ArgumentException("Last name cannot be empty.");
            if (!IsValidEmail(email))
                throw new ArgumentException("Email format is invalid. Please use a valid email (e.g., user@domain.com).");
            if (password.Length < 6)
                throw new ArgumentException("Password must be at least 6 characters long.");
        }

        // Validate credentials for email and password fields
        private void ValidateCredentials(string email, string password)
        {
            if (string.IsNullOrWhiteSpace(email))
                throw new ArgumentException("Email cannot be empty.");
            if (string.IsNullOrWhiteSpace(password))
                throw new ArgumentException("Password cannot be empty.");
        }

        // Create a new user instance based on the role (Admin or Client) with provided details
        private User CreateUser(int id, string email, string hashedPassword, string firstName, string lastName, Roles role)
        {
            return role == Roles.Admin
                ? new Admin
                {
                    Id = id,
                    Email = email,
                    Password = hashedPassword,
                    Name = firstName,
                    LastName = lastName,
                    Role = Roles.Admin
                }
                : new Client
                {
                    Id = id,
                    Email = email,
                    Password = hashedPassword,
                    Name = firstName,
                    LastName = lastName,
                    Role = Roles.User,
                    BankAccount = new Account()
                };
        }

        // check if the provided email is in a valid format using regex
        private bool IsValidEmail(string email)
        {
            try
            {
                var emailPattern = @"^[^\@\s]+@[^\@\s]+\.[^\@\s]+$";
                return Regex.IsMatch(email, emailPattern);
            }
            catch
            {
                return false;
            }
        }

        // Generate a new unique user ID by retrieving all users and finding the maximum ID
        private async Task<int> GenerateNewUserIdAsync()
        {
            var users = await _fileManager.GetAllUsersAsync();
            return users?.Count > 0 ? users.Max(u => u.Id) + 1 : 1;
        }
    }
}
