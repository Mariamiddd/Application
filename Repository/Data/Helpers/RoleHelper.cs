using System;
using System.Text.Json;
using Core.Enums;

namespace Repository.Data.Helpers
{
    // helper class to parse and convert user roles from JSON elements
    internal static class RoleHelper
    {
        /// Parse role from a JSON element property
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

        // parse role from string value 'admin' or 'client'
        private static bool TryParseStringRole(string? roleString, out bool isAdmin)
        {
            isAdmin = !string.IsNullOrEmpty(roleString) && 
                     roleString.Equals("admin", StringComparison.OrdinalIgnoreCase);
            return true;
        }

        // parse role from numeric value - 0=Admin, 1=User
        private static bool TryParseNumberRole(int roleValue, out bool isAdmin)
        {
            isAdmin = roleValue == (int)Roles.Admin;
            return true;
        }

        /// Convert Roles enum to JSON string representation
        public static string RoleToString(Roles role) =>
            role == Roles.Admin ? "admin" : "client";
    }
}
