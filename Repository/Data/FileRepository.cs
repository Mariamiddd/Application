using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Core.Interfaces;
using Core.Models;
using Repository.Data.Helpers;

namespace Repository.Data
{
    // FileRepository class implements the IFileManager interface to manage user and loan data using JSON files.
    public class FileRepository : IFileManager
    {
        private readonly string _usersFilePath;
        private readonly string _loansFilePath;

        public FileRepository()
        {
            // Determine the base directory for data files
            var dir = new DirectoryInfo(AppContext.BaseDirectory);
            while (dir != null && !File.Exists(Path.Combine(dir.FullName, "Application.slnx")))
            {
                dir = dir.Parent;
            }

            string dataDir;
            if (dir != null)
            {
                // If the solution file is found, set the data directory relative to it
                dataDir = Path.Combine(dir.FullName, "Repository", "Data");
            }
            else
            {
                // Fallback to current directory if solution file not found
                dataDir = Path.Combine(Directory.GetCurrentDirectory(), "Repository", "Data");
            }

            FileHelper.EnsureDirectoryExists(dataDir);

            _usersFilePath = Path.Combine(dataDir, "users.json");
            _loansFilePath = Path.Combine(dataDir, "loans.json");

            Console.WriteLine($"[FileRepository] Users file: {_usersFilePath}");
            Console.WriteLine($"[FileRepository] Loans file: {_loansFilePath}");
        }

        

        // Add a new user to the JSON file
        public async Task AddUserAsync(User user)
        {
            var users = await GetAllUsersAsync();
            users.Add(user);
            await SaveAllUsersAsync(users);
        }

        // Get all users from the JSON file
        public async Task<List<User>> GetAllUsersAsync()
        {
            if (!FileHelper.FileExists(_usersFilePath))
            {
                return new List<User>();
            }

            string json = await FileHelper.ReadFileAsync(_usersFilePath);
            return UserDeserializer.Deserialize(json);
        }

        // Get user by email
        public async Task<User?> GetUserByEmailAsync(string email)
        {
            var users = await GetAllUsersAsync();
            return users.FirstOrDefault(u => u.Email.Equals(email, StringComparison.OrdinalIgnoreCase));
        }

        // Get user by ID
        public async Task<User?> GetUserByIdAsync(int id)
        {
            var users = await GetAllUsersAsync();
            return users.FirstOrDefault(u => u.Id == id);
        }

        // Update an existing user
        public async Task UpdateUserAsync(User user)
        {
            var users = await GetAllUsersAsync();
            var index = users.FindIndex(u => u.Id == user.Id);
            if (index != -1)
            {
                users[index] = user;

                // Log the balance if the user is a Client
                if (user is Client client && client.BankAccount != null)
                {
                    Console.WriteLine($"[FileRepository] Saving user {user.Email} with balance: {client.BankAccount.Balance}");
                }

                await SaveAllUsersAsync(users);
            }
        }

        // Delete user by ID
        public async Task DeleteUserAsync(int id)
        {
            var users = await GetAllUsersAsync();
            users.RemoveAll(u => u.Id == id);
            await SaveAllUsersAsync(users);
        }

        // Save all users to file
        private async Task SaveAllUsersAsync(List<User> users)
        {
            try
            {
                string json = UserDeserializer.Serialize(users);
                bool success = await FileHelper.WriteFileAsync(_usersFilePath, json);

                if (success)
                {
                    Console.WriteLine($"[FileRepository] ✓ Saved {users.Count} users to {_usersFilePath}");
                }
                else
                {
                    Console.WriteLine($"[FileRepository] ✗ Error saving users");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[FileRepository] ✗ Error saving users: {ex.Message}");
            }
        }

        
        // Get all loan requests
        public async Task<List<LoanRequest>> GetAllLoanRequestsAsync()
        {
            if (!FileHelper.FileExists(_loansFilePath))
            {
                return new List<LoanRequest>();
            }

            string json = await FileHelper.ReadFileAsync(_loansFilePath);
            return LoanDeserializer.Deserialize(json);
        }

        // Get loan by ID
        public async Task<LoanRequest?> GetLoanRequestByIdAsync(int loanId)
        {
            var loans = await GetAllLoanRequestsAsync();
            return loans.FirstOrDefault(l => l.Id == loanId);
        }

        // Get loans by client ID
        public async Task<List<LoanRequest>> GetLoanRequestsByClientIdAsync(int clientId)
        {
            var loans = await GetAllLoanRequestsAsync();
            return loans.Where(l => l.ClientId == clientId).ToList();
        }

        // Get pending loan requests only
        public async Task<List<LoanRequest>> GetPendingLoanRequestsAsync()
        {
            var loans = await GetAllLoanRequestsAsync();
            return loans.Where(l => l.Status == Core.Enums.LoanStatus.Pending).ToList();
        }

        // Add a new loan request
        public async Task AddLoanRequestAsync(LoanRequest loanRequest)
        {
            var loans = await GetAllLoanRequestsAsync();

            // Generate new ID (1 if empty, otherwise max + 1)
            loanRequest.Id = loans.Count > 0 ? loans.Max(l => l.Id) + 1 : 1;

            loans.Add(loanRequest);
            await SaveAllLoansAsync(loans);
        }

        // Update an existing loan request
        public async Task UpdateLoanRequestAsync(LoanRequest loanRequest)
        {
            var loans = await GetAllLoanRequestsAsync();
            var index = loans.FindIndex(l => l.Id == loanRequest.Id);
            if (index != -1)
            {
                loans[index] = loanRequest;
                await SaveAllLoansAsync(loans);
            }
        }

        // Delete a loan request
        public async Task DeleteLoanRequestAsync(int loanId)
        {
            var loans = await GetAllLoanRequestsAsync();
            loans.RemoveAll(l => l.Id == loanId);
            await SaveAllLoansAsync(loans);
        }

        // Save all loans to file
        private async Task SaveAllLoansAsync(List<LoanRequest> loans)
        {
            try
            {
                string json = LoanDeserializer.Serialize(loans);
                bool success = await FileHelper.WriteFileAsync(_loansFilePath, json);

                if (success)
                {
                    Console.WriteLine($"[FileRepository] ✓ Saved {loans.Count} loans to {_loansFilePath}");
                }
                else
                {
                    Console.WriteLine($"[FileRepository] ✗ Error saving loans");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[FileRepository] ✗ Error saving loans: {ex.Message}");
            }
        }
    }
}
