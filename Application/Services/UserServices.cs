using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using Application.InterfaceServices;
using Core.Interfaces;
using Core.Models;

namespace Application.Services
{
    public class UserServices : IUserService
    {
        private readonly IFileManager _fileManager;

        public UserServices(IFileManager fileManager)
        {
            _fileManager = fileManager;
        }

        public List<User> GetAll()
        {
            return _fileManager.GetAllUsers();
        }

        public User? GetById(int id)
        {
            return _fileManager.GetUserById(id);
        }

        public User? GetByEmail(string email)
        {
            return _fileManager.GetUserByEmail(email);
        }
        public bool ExistsByEmail(string email)
        {
            // if iuser exists we return true else false
            return _fileManager.GetUserByEmail(email) != null;
        }

        public void Delete(int id)
        {
            var existingUser = GetById(id);
            if (existingUser != null)
            {
                throw new KeyNotFoundException($"User with id {id} not found.");
            }
            _fileManager.DeleteUser(id);
        }
        public void Update(User user)
        {
            var existingUser = GetById(user.Id);
            if (existingUser == null) {
                throw new KeyNotFoundException($"User with id {user.Id} not found.");
            }
            _fileManager.UpdateUser(user);
        }

    }
}
