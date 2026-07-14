using System;
using System.Collections.Generic;
using System.Reflection.Metadata;
using System.Text;
using Core.Models;

namespace Core.Interfaces
{
    // This interface defines the contract for a file manager that handles user data operations.
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
