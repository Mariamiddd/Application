using System;
using System.Threading.Tasks;
using Core.Models;
using Core.Interfaces;
using Core.Enums;

namespace UI
{
    internal class UserMenu
    {
        private readonly IFileManager _fileManager;
        private readonly UI.Interfaces.IConsole _console;

        public UserMenu(IFileManager fileManager, UI.Interfaces.IConsole console)
        {
            _fileManager = fileManager;
            _console = console;
        }

        // user menu for authenticated users

        // return true if user logs out, false otherwise
        public async Task<bool> ShowAsync(User user)
        {
            _console.Clear();
            _console.WriteLine($"Logged in as: {user.Email}");

            // shows the user menu 
            user.DisplayMenu();

            _console.Write("Option: ");
            var choice = _console.ReadLine();

            if (choice == "0") // logout
            {
                return true;
            }

            // is user a client? if so, cast to Client and perform actions
            if (user is Client client)
            {
                switch (choice)
                {
                    case "1":
                        _console.WriteLine($"Balance: {client.BankAccount.Balance:C}");
                        break;
                    case "2":
                        _console.Write("Amount to deposit: ");
                        if (decimal.TryParse(_console.ReadLine(), out var dep))
                        {
                            try
                            {
                                client.BankAccount.Deposit(dep);
                                await _fileManager.UpdateUserAsync(client);
                                _console.WriteLine("Deposit successful.");
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine($"Error: {ex.Message}");
                            }
                        }
                        else
                        {
                            _console.WriteLine("Invalid amount.");
                        }
                        break;
                    case "3":
                        _console.Write("Amount to withdraw: ");
                        if (decimal.TryParse(_console.ReadLine(), out var w))
                        {
                            try
                            {
                                var ok = client.BankAccount.Withdraw(w);
                                if (ok)
                                {
                                    await _fileManager.UpdateUserAsync(client);
                                    _console.WriteLine("Withdrawal successful.");
                                }
                                else
                                {
                                    Console.WriteLine("Insufficient funds.");
                                }
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine($"Error: {ex.Message}");
                            }
                        }
                        else
                        {
                            _console.WriteLine("Invalid amount.");
                        }
                        break;
                    case "4":
                        await ApplyForLoanAsync(client);
                        break;
                    case "5":
                        DisplayTransactionHistory(client);
                        break;
                    default:
                        Console.WriteLine("Unknown option.");
                        break;
                }
            }
            else
            {
                _console.WriteLine("User type not supported for client actions.");
            }

            _console.WriteLine("Press any key to continue...");
            _console.ReadKey(true);
            return false;
        }

        // method to apply for a loan
        private async Task ApplyForLoanAsync(Client client)
        {
            _console.WriteLine("\n=== Apply for Loan ===");

            _console.Write("Loan amount ($): ");
            if (!decimal.TryParse(_console.ReadLine(), out var loanAmount))
            {
                _console.WriteLine("Invalid amount.");
                return;
            }

            if (loanAmount <= 0)
            {
                _console.WriteLine("Loan amount must be greater than 0.");
                return;
            }

            _console.Write("Monthly income ($): ");
            if (!decimal.TryParse(_console.ReadLine(), out var monthlyIncome))
            {
                _console.WriteLine("Invalid income amount.");
                return;
            }

            if (monthlyIncome <= 0)
            {
                _console.WriteLine("Monthly income must be greater than 0.");
                return;
            }

            try
            {
                // Create a new loan request
                var loanRequest = new LoanRequest(client.Id, client.Name, loanAmount, monthlyIncome);

                // Add the loan request to the file manager for saving and future admin review
                await _fileManager.AddLoanRequestAsync(loanRequest);

                // Check if loan is safe 
                if (loanRequest.IsLoanSafe())
                {
                    _console.WriteLine($"\nLoan request of ${loanAmount} submitted successfully!");
                    _console.WriteLine("Your loan appears to be within safe lending parameters.");
                }
                else
                {
                    _console.WriteLine($"\nLoan request of ${loanAmount} submitted successfully!");
                    _console.WriteLine("Warning! Your loan may be at higher risk. An admin will review your application.");
                }
                _console.WriteLine("Status: Pending");
            }
            catch (Exception ex)
            {
                _console.WriteLine($"Error submitting loan request: {ex.Message}");
            }
        }

        private void DisplayTransactionHistory(Client client)
        {
            _console.Clear();
            _console.WriteLine("=== Transaction History ===\n");

            if (client.BankAccount.TransactionHistory == null || client.BankAccount.TransactionHistory.Count == 0)
            {
                _console.WriteLine("No transactions yet.");
            }
            else
            {
                foreach (var transaction in client.BankAccount.TransactionHistory)
                {
                    _console.WriteLine(transaction.ToString());
                }
            }

            _console.WriteLine("\nPress any key to continue...");
            _console.ReadKey(true);
        }
    }
}
