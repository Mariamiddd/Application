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

    // AuthService class implements IAuthService interface and provides methods for user registration, login, and password verification.
    public class AuthService : IAuthService
    {
        // because of readonly , the _fileManager field can only be set in the constructor and cannot be modified afterwards.
        private readonly IFileManager _fileManager;

        public AuthService(IFileManager fileManager)
        {
            if (fileManager == null)
            {
                throw new ArgumentNullException(nameof(fileManager));
            }
            _fileManager = fileManager;
        }

        public async Task<User> RegisterAsync(string email, string password, string firstName, string lastName, Roles role = Roles.User)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(email)) throw new ArgumentException("Email cannot be empty.");
                if (string.IsNullOrWhiteSpace(password)) throw new ArgumentException("Password cannot be empty.");
                if (string.IsNullOrWhiteSpace(firstName)) throw new ArgumentException("First name cannot be empty.");
                if (string.IsNullOrWhiteSpace(lastName)) throw new ArgumentException("Last name cannot be empty.");

                if (!IsValidEmail(email)) throw new ArgumentException("Email format is invalid. Please use a valid email (e.g., user@domain.com).");

                if (password.Length < 6) throw new ArgumentException("Password must be at least 6 characters long.");

                var existing = await _fileManager.GetUserByEmailAsync(email);
                if (existing != null) throw new InvalidOperationException("User with this email already exists.");

                User user;

                if (role == Roles.Admin)
                {
                    user = new Admin
                    {
                        Id = await GenerateNewUserIdAsync(),
                        Email = email,
                        Password = BCrypt.Net.BCrypt.HashPassword(password),
                        Name = firstName,
                        lastName = lastName,
                        Role = Roles.Admin
                    };
                }
                else
                {
                    var client = new Client
                    {
                        Id = await GenerateNewUserIdAsync(),
                        Email = email,
                        Password = BCrypt.Net.BCrypt.HashPassword(password),
                        Name = firstName,
                        lastName = lastName,
                        Role = Roles.User
                    };

                    if (client.BankAccount == null)
                    {
                        client.BankAccount = new Account();
                    }
                    user = client;
                }

                await _fileManager.AddUserAsync(user);
                return user;
            }
            catch (ArgumentException)
            {
                throw;
            }
            catch (InvalidOperationException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("An error occurred during user registration.", ex);
            }
        }

        // Validate email format using a regular expression.
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

        private async Task<int> GenerateNewUserIdAsync()
        {
            try
            {
                var users = await _fileManager.GetAllUsersAsync();
                if (users == null || users.Count == 0) return 1;
                return users.Max(u => u.Id) + 1;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("An error occurred while generating a new user ID.", ex);
            }
        }

        public async Task<bool> VerifyPasswordAsync(string email, string password)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(email)) throw new ArgumentException("Email cannot be empty.");
                if (string.IsNullOrWhiteSpace(password)) throw new ArgumentException("Password cannot be empty.");

                var user = await _fileManager.GetUserByEmailAsync(email);
                if (user == null) return false;

                return BCrypt.Net.BCrypt.Verify(password, user.Password);
            }
            catch (ArgumentException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("An error occurred during password verification.", ex);
            }
        }

        public async Task<User?> LoginAsync(string email, string password)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(email)) throw new ArgumentException("Email cannot be empty.");
                if (string.IsNullOrWhiteSpace(password)) throw new ArgumentException("Password cannot be empty.");

                var user = await _fileManager.GetUserByEmailAsync(email);
                if (user == null) return null;

                var isPasswordValid = BCrypt.Net.BCrypt.Verify(password, user.Password);
                if (!isPasswordValid) return null;

                return user;
            }
            catch (ArgumentException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("An error occurred during login.", ex);
            }
        }
    }
}
