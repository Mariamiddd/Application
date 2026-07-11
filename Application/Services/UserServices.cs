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


        public IReadOnlyList<UserModel> GetAll()
        {
            var users = _fileManager.GetAllUsers() ?? new List<Account>();
            var models = users.Select(a => MapToModel(a)).ToList();
            return (IReadOnlyList<UserModel>)models;
        }

        public UserModel? GetById(int id)
        {
            var account = _fileManager.GetUserById(id);
            if (account == null) return null;
            return MapToModel(account);
        }

        public UserModel? GetByEmail(string email)
        {
            var account = _fileManager.GetUserByEmail(email);
            if (account == null) return null;
            return MapToModel(account);
        }

        public void Update(int id, UserUpdateModel updateModel)
        {
            var account = _fileManager.GetUserById(id);
            if (account == null) throw new KeyNotFoundException("User not found.");

            if (updateModel.FirstName is not null) account.Name = updateModel.FirstName;
            if (updateModel.LastName is not null) account.lasName = updateModel.LastName;
            if (updateModel.IsVerified.HasValue) account.isVerified = updateModel.IsVerified.Value;

            _fileManager.UpdateUser(account);
        }

        public void Delete(int id)
        {
            _fileManager.DeleteUser(id);
        }

        public bool ExistsByEmail(string email)
        {
            var account = _fileManager.GetUserByEmail(email);
            return account != null;
        }

        private UserModel MapToModel(Account a)
        {
            return new UserModel(a.Id, a.Email ?? string.Empty, a.Name, a.lasName, a.isVerified);
        }

        // User creation and authentication responsibilities moved to AuthService
    }
}
