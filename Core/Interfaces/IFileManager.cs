using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Core.Models;

namespace Core.Interfaces
{
    // This interface defines the contract for a file manager that handles user data operations. (repository -> data -> filereppsitory)
    public interface IFileManager
    {
        Task<List<User>> GetAllUsersAsync();
        Task<User?> GetUserByIdAsync(int id);
        Task<User?> GetUserByEmailAsync(string email);

        Task AddUserAsync(User user);
        Task DeleteUserAsync(int id);
        Task UpdateUserAsync(User user);

        // Loan management methods
        Task<List<LoanRequest>> GetAllLoanRequestsAsync();
        Task<LoanRequest?> GetLoanRequestByIdAsync(int loanId);
        Task<List<LoanRequest>> GetLoanRequestsByClientIdAsync(int clientId);
        Task<List<LoanRequest>> GetPendingLoanRequestsAsync();
        Task AddLoanRequestAsync(LoanRequest loanRequest);
        Task UpdateLoanRequestAsync(LoanRequest loanRequest);
        Task DeleteLoanRequestAsync(int loanId);

    }
}
