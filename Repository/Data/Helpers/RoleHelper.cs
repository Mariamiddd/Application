using System;
using System.Text.Json;
using Core.Enums;

namespace Repository.Data.Helpers
{
    /// <summary>
    /// Helper class for parsing roles from different JSON value types
    /// </summary>
    internal static class RoleHelper
    {
        /// <summary>
        /// Parse role from a JSON element property
        /// </summary>
        public static bool TryParseRole(JsonElement roleProp, out bool isAdmin)
        {
            isAdmin = false;

            return roleProp.ValueKind switch
            {
                JsonValueKind.String => TryParseStringRole(roleProp.GetString(), out isAdmin),
                JsonValueKind.Number => TryParseNumberRole(roleProp.GetInt32(), out isAdmin),
                _ => false
            };
        }

        /// <summary>
        /// Parse role from string value (e.g., "admin", "client")
        /// </summary>
        private static bool TryParseStringRole(string? roleString, out bool isAdmin)
        {
            isAdmin = !string.IsNullOrEmpty(roleString) && 
                     roleString.Equals("admin", StringComparison.OrdinalIgnoreCase);
            return true;
        }

        /// <summary>
        /// Parse role from numeric value (legacy format: 0=Admin, 1=User)
        /// </summary>
        private static bool TryParseNumberRole(int roleValue, out bool isAdmin)
        {
            isAdmin = roleValue == (int)Roles.Admin;
            return true;
        }

        /// <summary>
        /// Convert Roles enum to JSON string representation
        /// </summary>
        public static string RoleToString(Roles role) =>
            role == Roles.Admin ? "admin" : "client";
    }
}
