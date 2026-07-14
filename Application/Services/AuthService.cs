using System;
using System.Globalization;
using System.Linq;
using System.Security.Cryptography;
using Application.InterfaceServices;
using Core.Interfaces;
using Core.Models;

namespace Application.Services
{
    public class AuthService : IAuthService
    {
        private readonly IFileManager _fileManager;
        
        public AuthService(IFileManager fileManager)
        {
            _fileManager = fileManager;
        }

        public User Register(string email, string password, string firstName, string lastName)
        {

            //validation
            if (string.IsNullOrWhiteSpace(email)) throw new ArgumentException("Email cannot be empty.");
            if (string.IsNullOrWhiteSpace(password)) throw new ArgumentException("Password cannot be empty.");
            if (string.IsNullOrWhiteSpace(firstName)) throw new ArgumentException("First name cannot be empty.");
            if (string.IsNullOrWhiteSpace(lastName)) throw new ArgumentException("Last name cannot be empty.");

            // check if there is duplicate
            var existing = _fileManager.GetUserByEmail(email);
            if (existing != null) throw new InvalidOperationException("User with this email already exists.");

            //create new  user and hash the password

            var client = new Client
            {
                Id = GenerateNewUserId(),
                Email = email,
                Password = BCrypt.Net.BCrypt.HashPassword(password),
                Name = firstName,
                lastName = lastName,
                isVerified = false
            };

            // ensure client has a bank account for balance tracking
            if (client.BankAccount == null)
            {
                client.BankAccount = new Account();
            }
            _fileManager.AddUser(client);
            return client;
        }

        //method to generate new user id 
        private int GenerateNewUserId()
        {
            var users = _fileManager.GetAllUsers();
            if (users == null || users.Count == 0) return 1; 
            return users.Max(u => u.Id) + 1;
        }

        //method to verify password
        public bool VerifyPassword(string email, string password)
        {
            // searching for user by email
            var user = _fileManager.GetUserByEmail(email);
            if (user == null) return false;

            return BCrypt.Net.BCrypt.Verify(password, user.Password);
        }
        public User? Login(string email, string password)
        {
            //search for user by email
            var user = _fileManager.GetUserByEmail(email);
            if (user == null) return null;

            var isPasswordValid = BCrypt.Net.BCrypt.Verify(password, user.Password);
            if (!isPasswordValid) return null;

            return user;
        }
    }
}
