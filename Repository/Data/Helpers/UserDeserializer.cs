using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using Core.Models;

namespace Repository.Data.Helpers
{
    /// <summary>
    /// Helper class for deserializing users from different JSON formats
    /// </summary>
    public static class UserDeserializer
    {
        private const string ArrayFormatStart = "[";
        private const string TypeProperty = "$type";
        private const string RoleProperty = "Role";
        private const string AdminType = "Admin";
        private const string ClientType = "Client";

        private static readonly JsonSerializerOptions Options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            WriteIndented = true,
            Converters = { new RoleJsonConverter() }
        };

        /// <summary>
        /// Deserialize users from JSON (handles both array and legacy formats)
        /// </summary>
        public static List<User> Deserialize(string json)
        {
            if (string.IsNullOrWhiteSpace(json))
                return new List<User>();

            try
            {
                return json.TrimStart().StartsWith(ArrayFormatStart)
                    ? DeserializeArrayFormat(json)
                    : DeserializeLegacyFormat(json);
            }
            catch
            {
                return new List<User>();
            }
        }

        /// <summary>
        /// Deserialize from array format (JSON array with Role field)
        /// </summary>
        private static List<User> DeserializeArrayFormat(string json)
        {
            try
            {
                using var doc = JsonDocument.Parse(json);
                return doc.RootElement.EnumerateArray()
                    .Select(DeserializeArrayElement)
                    .Where(u => u != null)
                    .Cast<User>()
                    .ToList();
            }
            catch
            {
                return new List<User>();
            }
        }

        /// <summary>
        /// Deserialize a single user from array element
        /// </summary>
        private static User? DeserializeArrayElement(JsonElement element)
        {
            try
            {
                if (!element.TryGetProperty(RoleProperty, out var roleProp) || 
                    !RoleHelper.TryParseRole(roleProp, out var isAdmin))
                    return null;

                var userJson = element.GetRawText();
                return isAdmin 
                    ? JsonSerializer.Deserialize<Admin>(userJson, Options)
                    : DeserializeClientWithAccount(userJson);
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Deserialize from legacy format (line-by-line with $type field)
        /// </summary>
        private static List<User> DeserializeLegacyFormat(string json)
        {
            return json.Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries)
                .Select(DeserializeLegacyLine)
                .Where(u => u != null)
                .Cast<User>()
                .ToList();
        }

        /// <summary>
        /// Deserialize a single user from legacy line format
        /// </summary>
        private static User? DeserializeLegacyLine(string line)
        {
            if (string.IsNullOrWhiteSpace(line))
                return null;

            try
            {
                using var doc = JsonDocument.Parse(line);

                if (!doc.RootElement.TryGetProperty(TypeProperty, out var typeProp))
                    return null;

                var type = typeProp.GetString() ?? string.Empty;
                return type == AdminType
                    ? JsonSerializer.Deserialize<Admin>(line, Options)
                    : type == ClientType
                        ? DeserializeClientWithAccount(line)
                        : null;
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Deserialize a Client and ensure BankAccount is initialized
        /// </summary>
        private static Client? DeserializeClientWithAccount(string json)
        {
            var client = JsonSerializer.Deserialize<Client>(json, Options);
            if (client?.BankAccount == null)
                client!.BankAccount = new Account();
            return client;
        }

        /// <summary>
        /// Serialize users to JSON array format
        /// </summary>
        public static string Serialize(List<User> users)
        {
            try
            {
                var userJsons = users
                    .Select(u => JsonSerializer.Serialize(u, u.GetType(), Options))
                    .ToList();

                var newLine = "\r\n";
                var json = "[" + newLine +
                          string.Join("," + newLine, userJsons.Select(j => "  " + j.TrimStart())) +
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
