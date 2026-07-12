using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Core.Enums;
using Core.Interfaces;
using Core.Models;


namespace Repository.Data
{

    // ifilemanager implementation in file repository
    public class FileRepository : IFileManager
    {
        private readonly string _filePath = Path.Combine(Directory.GetCurrentDirectory(), "users.json");

        private readonly JsonSerializerOptions _options = new JsonSerializerOptions
        {
            WriteIndented = true,
            ReferenceHandler = ReferenceHandler.IgnoreCycles
        };
        public void AddUser(User user)
        {
            var jsonObject = JsonSerializer.SerializeToElement(user, _options);
            var dict = JsonSerializer.Deserialize<Dictionary<string, object>>(jsonObject.GetRawText(), _options) ?? new Dictionary<string, object>();
            dict["$type"] = user.GetType().Name;

            var line = JsonSerializer.Serialize(dict, _options);
            File.AppendAllText(_filePath, line + Environment.NewLine);
        }

        public void DeleteUser(int id)
        {
            var users = GetAllUsers();
            users.RemoveAll(u => u.Id == id);
            SaveAll(users);
        }

        public List<User> GetAllUsers()
        {
            if (!File.Exists(_filePath))
            {
                return new List<User>();
            }

            string[] lines = File.ReadAllLines(_filePath);
            List<User> users = new List<User>();

            foreach (var line in lines)
            {
                if (string.IsNullOrWhiteSpace(line)) continue;

                var jsonDoc = JsonDocument.Parse(line);
                var type = jsonDoc.RootElement.GetProperty("$type").GetString();

                if (type == nameof(Client))
                    users.Add(JsonSerializer.Deserialize<Client>(line));
                else if (type == nameof(Admin))
                    users.Add(JsonSerializer.Deserialize<Admin>(line));
            }
            return users;
        }

        public User? GetUserByEmail(string email)
        {
            List<User> users = GetAllUsers();
            var user = users.FirstOrDefault(u => string.Equals(u.Email, email, StringComparison.OrdinalIgnoreCase));
            return user;
        }

        public User GetUserById(int id)
        {
            List<User> users = GetAllUsers();
            var user = users.FirstOrDefault(u => u.Id == id);
            return user;
        }

        public void RemoveUser(User user)
        {
            if (user == null) return;
            DeleteUser(user.Id);
        }

       

        public void SaveChanges()
        {
            
        }

        public void UpdateUser(User user)
        {
            var users = GetAllUsers();
            int index = users.FindIndex(u => u.Id == user.Id);
            if (index != -1)
            {
                users[index] = user;
                SaveAll(users);
            }

        }


        private void SaveAll(List<User> users)
        {
            var lines = new List<string>();
            foreach (var u in users)
            {
                var jsonObject = JsonSerializer.SerializeToElement(u, _options);
                var dict = JsonSerializer.Deserialize<Dictionary<string, object>>(jsonObject.GetRawText(), _options) ?? new Dictionary<string, object>();
                dict["$type"] = u.GetType().Name;
                lines.Add(JsonSerializer.Serialize(dict, _options));
            }
            File.WriteAllLines(_filePath, lines);
        }

    }
}
