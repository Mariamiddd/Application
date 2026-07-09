using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using Core.Interfaces;
using Core.Models;

namespace Application.Services
{
    public class UserServices
    {
        private readonly IFileManager _fileManager;
        public UserServices(IFileManager fileManager)
        {
            _fileManager = fileManager;
        }

        public void RegisterUser(string email, string password)
        {
            var existingUser = _fileManager.GetUserByEmail(email);
            if (existingUser != null)
            {
                throw new Exception("User already exists.");
            }
            var newUser = new Account
            {
                Id = GenerateNewUserId(),
                Email = email,
                Password = BCrypt.Net.BCrypt.HashPassword(password)
            };
            _fileManager.AddUser(newUser);
        }

        private int GenerateNewUserId()
        {
            var users = _fileManager.GetAllUsers();
            if (users == null || users.Count == 0) return 1;
            return users.Max(u => u.Id) + 1;
        }
    }
}
