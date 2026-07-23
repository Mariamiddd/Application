using System;
using System.Collections.Generic;
using System.Text.Json;
using Core.Models;

namespace Repository.Data.Helpers
{
    /// <summary>
    /// Helper class for deserializing loan requests from JSON
    /// </summary>
    public static class LoanDeserializer
    {
        private static readonly JsonSerializerOptions Options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            WriteIndented = true
        };

        /// <summary>
        /// Deserialize loans from JSON
        /// </summary>
        public static List<LoanRequest> Deserialize(string json)
        {
            if (string.IsNullOrWhiteSpace(json))
                return new List<LoanRequest>();

            try
            {
                return JsonSerializer.Deserialize<List<LoanRequest>>(json, Options) ?? new List<LoanRequest>();
            }
            catch
            {
                return new List<LoanRequest>();
            }
        }

        /// <summary>
        /// Serialize loans to JSON
        /// </summary>
        public static string Serialize(List<LoanRequest> loans)
        {
            try
            {
                return JsonSerializer.Serialize(loans, Options);
            }
            catch
            {
                return "[]";
            }
        }
    }
}
