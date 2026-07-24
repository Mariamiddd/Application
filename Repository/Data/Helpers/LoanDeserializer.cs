using System;
using System.Collections.Generic;
using System.Text.Json;
using Core.Models;

namespace Repository.Data.Helpers
{
    // loan deserializer class to handle serialization and deserialization of loan requests 
    public static class LoanDeserializer
    {
        private static readonly JsonSerializerOptions Options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            WriteIndented = true
        };

        //deserialize loans from JSON
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

        // serialize loans to JSON
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
