using System;
using System.Collections.Generic;
using System.Reflection.Metadata;
using System.Text;
using Core.Models;

namespace Core.Interfaces
{
    public interface IFileManager
    {
        List<Account> GetAllUsers();
        Account GetUserById(int id);
        Account GetUserByEmail(string email);

        void AddUser(Account user);
        void RemoveUser(Account user);
        void DeleteUser(int id);
        void UpdateUser(Account user);
        void SaveChanges();

    }
}
