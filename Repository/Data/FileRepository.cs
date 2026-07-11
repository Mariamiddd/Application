using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using Core.Interfaces;
using Core.Models;


namespace Repository.Data
{
    public class FileRepository : IFileManager
    {
        private readonly string _filePath = @"C:\Users\shiol\OneDrive\Desktop\Project\Application\Repository\Data\users.json";

        public void AddUser(Account user)
        {
            string line = JsonSerializer.Serialize(user);
            File.AppendAllText(_filePath, line + Environment.NewLine);
        }

        public void DeleteUser(int id)
        {
            var users = GetAllUsers();
            users.RemoveAll(u => u.Id == id);
            var lines = users.Select(u => JsonSerializer.Serialize(u)).ToArray();
            File.WriteAllLines(_filePath, lines);
        }

        public List<Account> GetAllUsers()
        {
            if (!File.Exists(_filePath))
            {
                return new List<Account>();
            }

            string[] lines = File.ReadAllLines(_filePath);
            List<Account> users = new List<Account>();
            foreach (var line in lines)
            {
                if (string.IsNullOrWhiteSpace(line))
                {
                    continue;
                }
                try
                {
                    Account? account = JsonSerializer.Deserialize<Account>(line);
                    if (account != null)
                    {
                        users.Add(account);
                    }
                }
                catch
                {
                   
                }
            }
            return users;
        }

        public Account? GetUserByEmail(string email)
        {
            List<Account> users = GetAllUsers();
            var user = users.FirstOrDefault(u => string.Equals(u.Email, email, StringComparison.OrdinalIgnoreCase));
            return user;
        }

        public Account? GetUserById(int id)
        {
            List<Account> users = GetAllUsers();
            var user = users.FirstOrDefault(u => u.Id == id);
            return user;
        }

        public void RemoveUser(Account user)
        {
            if (user == null) return;
            List<Account> users = GetAllUsers();
            users.RemoveAll(u => u.Id == user.Id);
            string[] lines = users.Select(u => JsonSerializer.Serialize(u)).ToArray();
            File.WriteAllLines(_filePath, lines);
        }

        public void SaveChanges()
        {
            
        }

        public void UpdateUser(Account user)
        {
            List<Account> users = GetAllUsers();
            int index = users.FindIndex(u => u.Id == user.Id);
            if (index != -1)
            {
                users[index] = user;

                string[] lines = users.Select(u => JsonSerializer.Serialize(u)).ToArray();
                File.WriteAllLines(_filePath, lines);
            }
        }
    }
}
