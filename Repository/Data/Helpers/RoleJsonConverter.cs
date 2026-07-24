using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using Core.Enums;

namespace Repository.Data.Helpers
{
    ///JSON converter for the Roles enum to serialize as strings instead of numbers
    public class RoleJsonConverter : JsonConverter<Roles>
    {
        public override Roles Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            return reader.TokenType switch
            {
                JsonTokenType.String => ParseStringRole(reader.GetString()),
                JsonTokenType.Number => ParseNumberRole(reader.GetInt32()),
                _ => throw new JsonException($"Unexpected token {reader.TokenType} when parsing role")
            };
        }

        // Serialize the Roles enum as a string
        public override void Write(Utf8JsonWriter writer, Roles value, JsonSerializerOptions options) =>
            writer.WriteStringValue(RoleHelper.RoleToString(value));

        // Parse a string role to the Roles enum
        private static Roles ParseStringRole(string? roleString) =>
            !string.IsNullOrEmpty(roleString) && roleString.Equals("admin", StringComparison.OrdinalIgnoreCase)
                ? Roles.Admin
                : Roles.User;

        // Parse a numeric role to the Roles enum
        private static Roles ParseNumberRole(int roleValue) =>
            roleValue == (int)Roles.Admin ? Roles.Admin : Roles.User;
    }
}
