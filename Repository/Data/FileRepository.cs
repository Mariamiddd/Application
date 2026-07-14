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
        // Try to resolve the repository data file inside the solution directory (Repository/Data/users.json).
        // If resolution fails, fall back to current directory.
        private readonly string _filePath;

        private readonly JsonSerializerOptions _options = new JsonSerializerOptions
        {
            // ensure each record is serialized to a single line so file can be read line-by-line
            WriteIndented = false,
            ReferenceHandler = ReferenceHandler.IgnoreCycles
        };
        public FileRepository()
        {
            // Attempt to locate the solution root by walking up from the runtime base directory.
            var dir = new System.IO.DirectoryInfo(AppContext.BaseDirectory!);
            while (dir != null && !System.IO.File.Exists(Path.Combine(dir.FullName, "Application.slnx")))
            {
                dir = dir.Parent;
            }

            if (dir != null)
            {
                var repoDir = Path.Combine(dir.FullName, "Repository", "Data");
                if (!Directory.Exists(repoDir)) Directory.CreateDirectory(repoDir);
                _filePath = Path.Combine(repoDir, "users.json");
            }
            else
            {
                // fallback to current directory
                var fallbackDir = Directory.GetCurrentDirectory();
                _filePath = Path.Combine(fallbackDir, "users.json");
            }
            Log($"FileRepository initialized. Using data file: {_filePath}");
        }

        public void AddUser(User user)
        {
            try
            {
                var raw = JsonSerializer.Serialize((object)user, user.GetType(), _options);
                Log($"AddUser raw serialization for id={user.Id}: {raw}");
            }
            catch (Exception ex)
            {
                Log($"AddUser raw serialize error: {ex}");
            }
            var line = SerializeWithType(user);
            Log($"AddUser final line for id={user.Id}: {line}");
            // ensure directory exists
            var folder = Path.GetDirectoryName(_filePath);
            if (!string.IsNullOrEmpty(folder) && !Directory.Exists(folder)) Directory.CreateDirectory(folder);
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

            foreach (var rawLine in lines)
            {
                if (string.IsNullOrWhiteSpace(rawLine)) continue;

                // Trim and remove BOM if present
                var line = rawLine.Trim();
                if (line.Length > 0 && line[0] == '\uFEFF') line = line.Substring(1);

                try
                {
                    using var jsonDoc = JsonDocument.Parse(line);
                    if (!jsonDoc.RootElement.TryGetProperty("$type", out var typeProp))
                    {
                        // missing type marker - skip
                        continue;
                    }

                    var type = typeProp.GetString();
                    if (type == nameof(Client))
                    {
                        var obj = JsonSerializer.Deserialize<Client>(line, _options);
                        if (obj != null)
                        {
                            if (obj.BankAccount == null) obj.BankAccount = new Core.Models.Account();
                            users.Add(obj);
                        }
                    }
                    else if (type == nameof(Admin))
                    {
                        var obj = JsonSerializer.Deserialize<Admin>(line, _options);
                        if (obj != null) users.Add(obj);
                    }
                    // unknown type -> skip
                }
                catch (JsonException)
                {
                    // skip malformed line
                    continue;
                }
                catch
                {
                    // skip any unexpected error for robustness
                    continue;
                }
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
            // Persist current users to the file. This rewrites the entire file atomically.
            try
            {
                var users = GetAllUsers();
                SaveAll(users);
            }
            catch
            {
                // swallow to preserve previous behavior; callers may handle errors
            }
        }

        public void UpdateUser(User user)
        {
            var users = GetAllUsers();
            int index = users.FindIndex(u => u.Id == user.Id);
            if (index != -1)
            {
                users[index] = user;
                try
                {
                    var debugJson = SerializeWithType(user);
                    Log($"UpdateUser writing user id={user.Id}: {debugJson}");
                }
                catch (Exception ex)
                {
                    Log($"UpdateUser serialize error: {ex}");
                }
                SaveAll(users);
            }

        }


        private void SaveAll(List<User> users)
        {
            var lines = new List<string>();
            if (users == null || users.Count == 0)
            {
                Log("SaveAll called with empty users list; skipping file write to avoid data loss.");
                return;
            }
            foreach (var u in users)
            {
                lines.Add(SerializeWithType(u));
            }
            try
            {
                File.WriteAllLines(_filePath, lines);
            }
            catch (Exception ex)
            {
                Log($"Failed to write all users to file: {ex}");
            }
        }

        private string SerializeWithType(User user)
        {
            // serialize user to JSON, then write a new object merging all properties and adding $type
            var json = JsonSerializer.Serialize((object)user, user.GetType(), _options);
            using var doc = JsonDocument.Parse(json);
            using var ms = new MemoryStream();
            using (var writer = new Utf8JsonWriter(ms, new JsonWriterOptions { Indented = false }))
            {
                writer.WriteStartObject();
                foreach (var prop in doc.RootElement.EnumerateObject())
                {
                    prop.WriteTo(writer);
                }
                writer.WriteString("$type", user.GetType().Name);
                writer.WriteEndObject();
            }
            return Encoding.UTF8.GetString(ms.ToArray());
        }

        private void Log(string message)
        {
            try
            {
                var folder = Path.GetDirectoryName(_filePath) ?? Directory.GetCurrentDirectory();
                var logPath = Path.Combine(folder, "file_repo.log");
                var line = $"[{DateTime.UtcNow:O}] {message}{Environment.NewLine}";
                File.AppendAllText(logPath, line);
            }
            catch
            {
                // ignore logging errors
            }
        }

    }
}
