using System;
using Core.Models;
using Core.Interfaces;

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

        // Show client menu. Returns true if user chose to logout.
        public bool Show(User user)
        {
            _console.Clear();
            _console.WriteLine($"Logged in as: {user.Email}");

            // Use polymorphic display if available
            user.DisplayMenu();

            _console.Write("Option: ");
            var choice = _console.ReadLine();

            if (choice == MenuKeys.Logout) // logout for client
            {
                return true;
            }

            // Handle a few basic client actions using the file manager
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
                                _fileManager.UpdateUser(client);
                                _fileManager.SaveChanges();
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
                                    _fileManager.UpdateUser(client);
                                    _fileManager.SaveChanges();
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
                        Console.WriteLine("Loan application feature not implemented in UI yet.");
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
    }
}
