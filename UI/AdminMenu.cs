using System;
using System.Threading.Tasks;
using Core.Models;
using Core.Interfaces;
using Core.Enums;

namespace UI
{
    internal class AdminMenu
    {
        private readonly Interfaces.IConsole _console;
        private readonly IFileManager _fileManager;

        public AdminMenu(Interfaces.IConsole console, IFileManager fileManager)
        {
            _console = console;
            _fileManager = fileManager;
        }

        // Show menu for any logged-in user. If returns true, request logout.
        public async Task<bool> ShowAsync(User user)
        {
            // Clear the console and display the logged-in user's email
            _console.Clear();
            _console.WriteLine($"Logged in as: {user.Email}");

            // Display the menu options based on the user's role
            user.DisplayMenu();

            _console.Write("Option: ");
            var choice = _console.ReadLine();

            // Basic logout handling same for both admin and client
            if (choice == "0")
            {
                return true;
            }

            // Handle admin options
            if (user is Admin admin)
            {
                switch (choice)
                {
                    case "1":
                        await ProcessLoanRequestsAsync(admin);
                        break;
                    case "2":
                        await DeleteUserAccountAsync();
                        break;
                    default:
                        _console.WriteLine("Unknown option.");
                        break;
                }
            }
            else
            {
                _console.WriteLine("User type not supported for admin actions.");
            }

            _console.WriteLine("Press any key to continue...");
            _console.ReadKey(true);
            return false;
        }

        private async Task ProcessLoanRequestsAsync(Admin admin)
        {
            _console.Clear();
            _console.WriteLine("=== Process Loan Requests ===\n");

            // Get all pending loan requests
            try
            {
                var pendingLoans = await _fileManager.GetPendingLoanRequestsAsync();

                if (pendingLoans.Count == 0)
                {
                    _console.WriteLine("No pending loan requests at this time.");
                    _console.WriteLine("Press any key to continue...");
                    _console.ReadKey(true);
                    return;
                }

                _console.WriteLine($"Found {pendingLoans.Count} pending loan request(s):\n");

                // Display all pending loans
                for (int i = 0; i < pendingLoans.Count; i++)
                {
                    var loan = pendingLoans[i];
                    _console.WriteLine($"{i + 1}. Client: {loan.ClientName} | Amount: ${loan.Amount} | Income: ${loan.Income}");
                    _console.WriteLine($"   Status: {loan.Status} | Submitted: {loan.RequestDate:MM/dd/yyyy}");
                }

                _console.WriteLine($"\nSelect loan to process (1-{pendingLoans.Count}) or 0 to cancel: ");
                _console.Write("Option: ");

                if (!int.TryParse(_console.ReadLine(), out int choice) || choice < 0 || choice > pendingLoans.Count)
                {
                    _console.WriteLine("Invalid selection.");
                    return;
                }

                if (choice == 0)
                {
                    return;
                }

                var selectedLoan = pendingLoans[choice - 1];
                await ApproveLoanDialogAsync(selectedLoan);
            }
            catch (Exception ex)
            {
                _console.WriteLine($"Error processing loan requests: {ex.Message}");
            }
        }

        private async Task ApproveLoanDialogAsync(LoanRequest loan)
        {
            _console.Clear();
            _console.WriteLine("=== Loan Review ===\n");
            _console.WriteLine($"Client Name: {loan.ClientName}");
            _console.WriteLine($"Loan Amount: ${loan.Amount}");
            _console.WriteLine($"Monthly Income: ${loan.Income}");
            _console.WriteLine($"Request Date: {loan.RequestDate:MM/dd/yyyy HH:mm}");

            // Check if loan is safe
            bool isSafe = loan.IsLoanSafe();
            _console.WriteLine($"Loan Safety: {(isSafe ? "SAFE" : "RISKY")}");
            _console.WriteLine($"\nStatus: {loan.Status}\n");

            _console.WriteLine("1) Approve Loan");
            _console.WriteLine("2) Reject Loan");
            _console.WriteLine("0) Cancel");
            _console.Write("Option: ");

            var choice = _console.ReadLine();

            switch (choice)
            {
                case "1":
                    await ApproveLoanAsync(loan);
                    break;
                case "2":
                    await RejectLoanAsync(loan);
                    break;
                case "0":
                    return;
                default:
                    _console.WriteLine("Invalid selection.");
                    break;
            }
        }

        private async Task ApproveLoanAsync(LoanRequest loan)
        {
            try
            {
                // Update loan status
                loan.Status = LoanStatus.Approved;
                await _fileManager.UpdateLoanRequestAsync(loan);

                // Get the client and add the loan amount to their balance
                var client = await _fileManager.GetUserByIdAsync(loan.ClientId) as Client;
                if (client != null)
                {
                    client.BankAccount.Deposit(loan.Amount);
                    await _fileManager.UpdateUserAsync(client);
                }

                _console.WriteLine($"\n✓ Loan of ${loan.Amount} APPROVED for {loan.ClientName}");
                _console.WriteLine($"Amount added to client's balance.");
            }
            catch (Exception ex)
            {
                _console.WriteLine($"Error approving loan: {ex.Message}");
            }
        }

        private async Task RejectLoanAsync(LoanRequest loan)
        {
            try
            {
                // Update loan status
                loan.Status = LoanStatus.Rejected;
                await _fileManager.UpdateLoanRequestAsync(loan);

                _console.WriteLine($"\n✗ Loan of ${loan.Amount} REJECTED for {loan.ClientName}");
                _console.WriteLine($"Client has been notified.");
            }
            catch (Exception ex)
            {
                _console.WriteLine($"Error rejecting loan: {ex.Message}");
            }
        }

        private async Task DeleteUserAccountAsync()
        {
            _console.Clear();
            _console.WriteLine("=== Delete User Account ===\n");

            _console.Write("Enter user email to delete: ");
            string userEmail = (_console.ReadLine() ?? string.Empty).Trim();

            if (string.IsNullOrEmpty(userEmail))
            {
                _console.WriteLine("Invalid email. Operation cancelled.");
                _console.WriteLine("Press any key to continue...");
                _console.ReadKey(true);
                return;
            }

            try
            {
                // Retrieve user by email
                var userToDelete = await _fileManager.GetUserByEmailAsync(userEmail);

                if (userToDelete == null)
                {
                    _console.WriteLine($"User with email '{userEmail}' not found.");
                    _console.WriteLine("Press any key to continue...");
                    _console.ReadKey(true);
                    return;
                }

                // Display user details
                _console.WriteLine($"\nUser to Delete:");
                _console.WriteLine($"Name: {userToDelete.Name} {userToDelete.lastName}");
                _console.WriteLine($"Email: {userToDelete.Email}");
                _console.WriteLine($"Role: {userToDelete.Role}");

                // Warning for admin deletion
                if (userToDelete.Role == Roles.Admin)
                {
                    _console.WriteLine("\n⚠ WARNING: You are about to delete another ADMIN account.");
                }

                // Security confirmation - require email re-entry
                _console.WriteLine("\n--- Security Confirmation ---");
                _console.Write("Re-enter email to confirm deletion: ");
                string confirmationEmail = (_console.ReadLine() ?? string.Empty).Trim();

                if (confirmationEmail != userEmail)
                {
                    _console.WriteLine("\n✗ Email confirmation failed. Deletion cancelled.");
                    _console.WriteLine("Press any key to continue...");
                    _console.ReadKey(true);
                    return;
                }

                // Final confirmation
                _console.WriteLine("\nThis action CANNOT be undone!");
                _console.Write("Type 'YES' to confirm deletion: ");
                string finalConfirmation = (_console.ReadLine() ?? string.Empty).Trim().ToUpper();

                if (finalConfirmation != "YES")
                {
                    _console.WriteLine("\n✗ Deletion cancelled.");
                    _console.WriteLine("Press any key to continue...");
                    _console.ReadKey(true);
                    return;
                }

                // Perform deletion
                await _fileManager.DeleteUserAsync(userToDelete.Id);

                _console.WriteLine($"\n✓ User account '{userEmail}' has been successfully deleted.");
            }
            catch (Exception ex)
            {
                _console.WriteLine($"Error deleting user: {ex.Message}");
            }

            _console.WriteLine("Press any key to continue...");
            _console.ReadKey(true);
        }
    }
}
