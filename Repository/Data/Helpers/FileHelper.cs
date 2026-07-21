using System;
using System.IO;
using System.Threading.Tasks;

namespace Repository.Data.Helpers
{
    // helper class for file operations, providing methods to read, write, and check files safely.
    public static class FileHelper
    {
        // Read file content safely
        public static async Task<string> ReadFileAsync(string filePath)
        {
            try
            {
                return await File.ReadAllTextAsync(filePath);
            }
            catch
            {
                return string.Empty;
            }
        }

        // Write file content safely
        public static async Task<bool> WriteFileAsync(string filePath, string content)
        {
            try
            {
                await File.WriteAllTextAsync(filePath, content);
                return true;
            }
            catch
            {
                return false;
            }
        }

        // Check if file exists
        public static bool FileExists(string filePath)
        {
            return File.Exists(filePath);
        }

        // Create directory if it doesn't exist
        public static void EnsureDirectoryExists(string directoryPath)
        {
            if (!Directory.Exists(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);
            }
        }
    }
}
