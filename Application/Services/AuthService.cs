using System;
using System.Linq;
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

        public UserModel Register(RegistrationModel registrationModel)
        {
            if (string.IsNullOrWhiteSpace(registrationModel.Email)) throw new ArgumentException("Email is required", nameof(registrationModel.Email));
            if (string.IsNullOrWhiteSpace(registrationModel.Password)) throw new ArgumentException("Password is required", nameof(registrationModel.Password));

            var existing = _fileManager.GetUserByEmail(registrationModel.Email);
            if (existing != null) throw new InvalidOperationException("User already exists.");

            var account = new Account
            {
                Id = GenerateNewUserId(),
                Email = registrationModel.Email,
                Password = BCrypt.Net.BCrypt.HashPassword(registrationModel.Password),
                Name = registrationModel.FirstName ?? string.Empty,
                lasName = registrationModel.LastName ?? string.Empty,
                isVerified = false
            };

            _fileManager.AddUser(account);
            var result = new UserModel(account.Id, account.Email ?? string.Empty, account.Name, account.lasName, account.isVerified);
            return result;
        }

        public bool VerifyPassword(string email, string password)
        {
            var account = _fileManager.GetUserByEmail(email);
            if (account == null) return false;
            var ok = BCrypt.Net.BCrypt.Verify(password, account.Password);
            return ok;
        }

        public UserModel? Login(string email, string password)
        {
            var account = _fileManager.GetUserByEmail(email);
            if (account == null) return null;
            var ok = BCrypt.Net.BCrypt.Verify(password, account.Password);
            if (!ok) return null;
            return new UserModel(account.Id, account.Email ?? string.Empty, account.Name, account.lasName, account.isVerified);
        }

        private int GenerateNewUserId()
        {
            var users = _fileManager.GetAllUsers();
            if (users == null || users.Count == 0) return 1;
            return users.Max(u => u.Id) + 1;
        }
    }
}
