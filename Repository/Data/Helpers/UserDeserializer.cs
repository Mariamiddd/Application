using System;
using System.Collections.Generic;
using System.Text.Json;
using Core.Enums;
using Core.Models;

namespace Repository.Data.Helpers
{
    // Helper class for deserializing users from different JSON formats
    public static class UserDeserializer
    {
        private const string ARRAY_FORMAT_START = "[";
        private const string TYPE_PROPERTY = "$type";
        private const string ROLE_PROPERTY = "Role";
        private const string ADMIN_TYPE = "Admin";
        private const string CLIENT_TYPE = "Client";

        private static readonly JsonSerializerOptions Options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            WriteIndented = true,
            Converters = { new RoleJsonConverter() }
        };

        // Main method: deserialize users from JSON (handles both formats)
        public static List<User> Deserialize(string json)
        {
            if (string.IsNullOrWhiteSpace(json))
            {
                return new List<User>();
            }

            try
            {
                // Check which format the file is in
                if (json.TrimStart().StartsWith(ARRAY_FORMAT_START))
                {
                    return DeserializeArrayFormat(json);
                }
                else
                {
                    return DeserializeLegacyFormat(json);
                }
            }
            catch
            {
                return new List<User>();
            }
        }

        // Deserialize from new format (JSON array with Role field)
        private static List<User> DeserializeArrayFormat(string json)
        {
            var users = new List<User>();

            try
            {
                using var doc = JsonDocument.Parse(json);

                foreach (var element in doc.RootElement.EnumerateArray())
                {
                    var user = DeserializeArrayElement(element);
                    if (user != null)
                    {
                        users.Add(user);
                    }
                }
            }
            catch
            {
                // Return what we have if parsing fails
            }

            return users;
        }

        // Deserialize a single user from array element
        private static User? DeserializeArrayElement(JsonElement element)
        {
            try
            {
                if (!element.TryGetProperty(ROLE_PROPERTY, out var roleProp))
                {
                    return null;
                }

                string userJson = element.GetRawText();
                bool isAdmin = false;

                // Handle both string and integer role values
                if (roleProp.ValueKind == JsonValueKind.String)
                {
                    // New format: string values like "admin" or "client"
                    string roleString = roleProp.GetString() ?? string.Empty;
                    isAdmin = roleString.Equals("admin", StringComparison.OrdinalIgnoreCase);
                }
                else if (roleProp.ValueKind == JsonValueKind.Number)
                {
                    // Legacy format: integer values (0 = Admin, 1 = User)
                    int roleValue = roleProp.GetInt32();
                    isAdmin = roleValue == (int)Roles.Admin;
                }
                else
                {
                    return null;
                }

                // Check role to determine type
                if (isAdmin)
                {
                    return JsonSerializer.Deserialize<Admin>(userJson, Options);
                }
                else
                {
                    var client = JsonSerializer.Deserialize<Client>(userJson, Options);
                    if (client != null && client.BankAccount == null)
                    {
                        client.BankAccount = new Account();
                    }
                    return client;
                }
            }
            catch
            {
                return null;
            }
        }

        // Deserialize from legacy format (line-by-line with $type field)
        private static List<User> DeserializeLegacyFormat(string json)
        {
            var users = new List<User>();
            var lines = json.Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);

            foreach (var line in lines)
            {
                if (string.IsNullOrWhiteSpace(line))
                {
                    continue;
                }

                var user = DeserializeLegacyLine(line);
                if (user != null)
                {
                    users.Add(user);
                }
            }

            return users;
        }

        // Deserialize a single user from legacy line format
        private static User? DeserializeLegacyLine(string line)
        {
            try
            {
                using var doc = JsonDocument.Parse(line);

                if (!doc.RootElement.TryGetProperty(TYPE_PROPERTY, out var typeProp))
                {
                    return null;
                }

                string type = typeProp.GetString() ?? string.Empty;

                // Check type to determine class
                if (type == CLIENT_TYPE)
                {
                    var client = JsonSerializer.Deserialize<Client>(line, Options);
                    if (client != null && client.BankAccount == null)
                    {
                        client.BankAccount = new Account();
                    }
                    return client;
                }
                else if (type == ADMIN_TYPE)
                {
                    return JsonSerializer.Deserialize<Admin>(line, Options);
                }
            }
            catch
            {
                // Skip lines that can't be deserialized
            }

            return null;
        }

        // Serialize users to JSON array format
        public static string Serialize(List<User> users)
        {
            try
            {
                // Serialize each user individually to preserve all properties
                var userJsonList = users.Select(user => 
                    JsonSerializer.Serialize(user, user.GetType(), Options)
                ).ToList();

                // Create formatted JSON array with consistent line endings (Windows CR LF)
                string newLine = "\r\n";
                string json = "[" + newLine + 
                              string.Join("," + newLine, userJsonList.Select(j => "  " + j.TrimStart())) + 
                              newLine + "]";

                return json;
            }
            catch
            {
                return "[]";
            }
        }
    }
}
