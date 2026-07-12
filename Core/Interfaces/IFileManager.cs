using System;
using System.Collections.Generic;
using System.Reflection.Metadata;
using System.Text;
using Core.Models;

namespace Core.Interfaces
{
    public interface IFileManager
    {
        List<User> GetAllUsers();
        User GetUserById(int id);
        User GetUserByEmail(string email);

        void AddUser(User user);
        void DeleteUser(int id);
        void UpdateUser(User user);
        void SaveChanges();

    }
}
