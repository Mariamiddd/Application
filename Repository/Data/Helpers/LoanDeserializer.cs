using System;
using System.Collections.Generic;
using System.Text.Json;
using Core.Models;

namespace Repository.Data.Helpers
{
    // Helper class for deserializing loan requests from JSON
    public static class LoanDeserializer
    {
        private static readonly JsonSerializerOptions Options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            WriteIndented = true
        };

        // Deserialize loans from JSON
        public static List<LoanRequest> Deserialize(string json)
        {
            if (string.IsNullOrWhiteSpace(json))
            {
                return new List<LoanRequest>();
            }

            try
            {
                var loans = JsonSerializer.Deserialize<List<LoanRequest>>(json, Options) 
                    ?? new List<LoanRequest>();
                return loans;
            }
            catch
            {
                return new List<LoanRequest>();
            }
        }

        // Serialize loans to JSON
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
