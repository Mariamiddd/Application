using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using Core.Enums;

namespace Repository.Data.Helpers
{
    /// <summary>
    /// Custom JSON converter for the Roles enum to serialize as strings instead of numbers
    /// </summary>
    public class RoleJsonConverter : JsonConverter<Roles>
    {
        public override Roles Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            switch (reader.TokenType)
            {
                case JsonTokenType.String:
                    {
                        string roleString = reader.GetString() ?? string.Empty;
                        return roleString.Equals("admin", StringComparison.OrdinalIgnoreCase) 
                            ? Roles.Admin 
                            : Roles.User;
                    }
                case JsonTokenType.Number:
                    {
                        int roleValue = reader.GetInt32();
                        return roleValue == (int)Roles.Admin ? Roles.Admin : Roles.User;
                    }
                default:
                    throw new JsonException($"Unexpected token {reader.TokenType} when parsing role");
            }
        }

        public override void Write(Utf8JsonWriter writer, Roles value, JsonSerializerOptions options)
        {
            // Serialize enum as string: Admin -> "admin", User -> "client"
            string roleString = value == Roles.Admin ? "admin" : "client";
            writer.WriteStringValue(roleString);
        }
    }
}
