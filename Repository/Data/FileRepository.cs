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
    // This class implements the IFileManager interface to manage user and loan data stored in JSON files.
    public class FileRepository : IFileManager
    {
        private readonly string _usersFilePath;
        private readonly string _loansFilePath;
        private const string SolutionFile = "Application.slnx";
        private const string DataFolder = "Data";
        private const string UsersFile = "users.json";
        private const string LoansFile = "loans.json";

        // Constructor initializes file paths and ensures the data directory exists
        public FileRepository()
        {
            var dataDir = FindDataDirectory();
            FileHelper.EnsureDirectoryExists(dataDir);

            _usersFilePath = Path.Combine(dataDir, UsersFile);
            _loansFilePath = Path.Combine(dataDir, LoansFile);

            Console.WriteLine($"[FileRepository] Users file: {_usersFilePath}");
            Console.WriteLine($"[FileRepository] Loans file: {_loansFilePath}");
        }

        // finddatadirectory method to locate the data directory based on the solution file's location
        private string FindDataDirectory()
        {
            var dir = new DirectoryInfo(AppContext.BaseDirectory);
            while (dir != null && !File.Exists(Path.Combine(dir.FullName, SolutionFile)))
                dir = dir.Parent;

            if (dir != null)
                return Path.Combine(dir.FullName, "Repository", DataFolder);

            return Path.Combine(Directory.GetCurrentDirectory(), "Repository", DataFolder);
        }


        // manage users add, update, delete, get by id, get by email
        public async Task AddUserAsync(User user)
        {
            var users = await GetAllUsersAsync();
            users.Add(user);
            await SaveUsersAsync(users);
        }

        public async Task<List<User>> GetAllUsersAsync()
        {
            if (!FileHelper.FileExists(_usersFilePath))
                return new List<User>();

            var json = await FileHelper.ReadFileAsync(_usersFilePath);
            return UserDeserializer.Deserialize(json);
        }

        public async Task<User?> GetUserByEmailAsync(string email) =>
            (await GetAllUsersAsync()).FirstOrDefault(u => u.Email.Equals(email, StringComparison.OrdinalIgnoreCase));

        public async Task<User?> GetUserByIdAsync(int id) =>
            (await GetAllUsersAsync()).FirstOrDefault(u => u.Id == id);

        public async Task UpdateUserAsync(User user)
        {
            var users = await GetAllUsersAsync();
            var index = users.FindIndex(u => u.Id == user.Id);

            if (index != -1)
            {
                users[index] = user;
                if (user is Client client && client.BankAccount != null)
                    Console.WriteLine($"[FileRepository] Saving user {user.Email} with balance: {client.BankAccount.Balance}");

                await SaveUsersAsync(users);
            }
        }

        public async Task DeleteUserAsync(int id)
        {
            var users = await GetAllUsersAsync();
            users.RemoveAll(u => u.Id == id);
            await SaveUsersAsync(users);
        }

        private async Task SaveUsersAsync(List<User> users) =>
            await SaveAsync(users, _usersFilePath, UserDeserializer.Serialize, "users");


        // manage loan requests add, update, delete, get by id, get by client id, get pending
        public async Task<List<LoanRequest>> GetAllLoanRequestsAsync()
        {
            if (!FileHelper.FileExists(_loansFilePath))
                return new List<LoanRequest>();

            var json = await FileHelper.ReadFileAsync(_loansFilePath);
            return LoanDeserializer.Deserialize(json);
        }

        public async Task<LoanRequest?> GetLoanRequestByIdAsync(int loanId) =>
            (await GetAllLoanRequestsAsync()).FirstOrDefault(l => l.Id == loanId);

        public async Task<List<LoanRequest>> GetLoanRequestsByClientIdAsync(int clientId) =>
            (await GetAllLoanRequestsAsync()).Where(l => l.ClientId == clientId).ToList();

        public async Task<List<LoanRequest>> GetPendingLoanRequestsAsync() =>
            (await GetAllLoanRequestsAsync()).Where(l => l.Status == Core.Enums.LoanStatus.Pending).ToList();

        public async Task AddLoanRequestAsync(LoanRequest loanRequest)
        {
            var loans = await GetAllLoanRequestsAsync();
            loanRequest.Id = loans.Count > 0 ? loans.Max(l => l.Id) + 1 : 1;
            loans.Add(loanRequest);
            await SaveLoansAsync(loans);
        }

        public async Task UpdateLoanRequestAsync(LoanRequest loanRequest)
        {
            var loans = await GetAllLoanRequestsAsync();
            var index = loans.FindIndex(l => l.Id == loanRequest.Id);

            if (index != -1)
            {
                loans[index] = loanRequest;
                await SaveLoansAsync(loans);
            }
        }

        public async Task DeleteLoanRequestAsync(int loanId)
        {
            var loans = await GetAllLoanRequestsAsync();
            loans.RemoveAll(l => l.Id == loanId);
            await SaveLoansAsync(loans);
        }

        private async Task SaveLoansAsync(List<LoanRequest> loans) =>
            await SaveAsync(loans, _loansFilePath, LoanDeserializer.Serialize, "loans");


        // save method to handle serialization and writing to file for both users and loans
        private async Task SaveAsync<T>(List<T> items, string filePath, Func<List<T>, string> serializer, string itemType)
        {
            try
            {
                var json = serializer(items);
                var success = await FileHelper.WriteFileAsync(filePath, json);

                if (success)
                    Console.WriteLine($"[FileRepository] ✓ Saved {items.Count} {itemType} to {filePath}");
                else
                    Console.WriteLine($"[FileRepository] ✗ Error saving {itemType}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[FileRepository] ✗ Error saving {itemType}: {ex.Message}");
            }
        }
    }
}
