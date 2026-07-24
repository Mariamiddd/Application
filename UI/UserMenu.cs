using System;
using System.Threading.Tasks;
using Core.Models;
using Core.Interfaces;
using Core.Enums;
using Spectre.Console;

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

        // user menu for logged in users

        // return true if user logs out, false otherwise
        public async Task<bool> ShowAsync(User user)
        {
            _console.Clear();

            var headerPanel = new Panel($"[bold cyan]Logged in as: {user.Email}[/]")
                .Header("[bold magenta]Client Dashboard[/]", Justify.Center)
                .RoundedBorder()
                .BorderColor(Color.Magenta)
                .Padding(1, 1);

            AnsiConsole.Write(headerPanel);

            // Display styled menu options
            var menuContent = "[bold cyan]1[/] Check Balance\n[bold cyan]2[/] Deposit Funds\n[bold cyan]3[/] Withdraw Funds\n[bold cyan]4[/] Apply for Loan\n[bold cyan]5[/] View Transaction History\n[bold cyan]0[/] Logout";
            var menuPanel = new Panel(menuContent)
                .Header("[bold magenta]Client Options[/]", Justify.Center)
                .RoundedBorder()
                .BorderColor(Color.Magenta)
                .Padding(1, 1);
            AnsiConsole.Write(menuPanel);

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
                        var balancePanel = new Panel($"[bold green]${client.BankAccount.Balance:F2}[/]")
                            .Header("[bold cyan]Account Balance[/]", Justify.Center)
                            .RoundedBorder()
                            .BorderColor(Color.Green)
                            .Padding(1, 1);
                        AnsiConsole.Write(balancePanel);
                        break;
                    case "2":
                        await DepositAsync(client);
                        break;
                    case "3":
                        await WithdrawAsync(client);
                        break;
                    case "4":
                        await ApplyForLoanAsync(client);
                        break;
                    case "5":
                        DisplayTransactionHistory(client);
                        break;
                    default:
                        _console.WriteLine("Unknown option.");
                        break;
                }
            }
            else
            {
                AnsiConsole.MarkupLine("[red]✗ User type not supported for client actions.[/]");
            }

            AnsiConsole.MarkupLine("[yellow]Press any key to continue...[/]");
            _console.ReadKey(true);
            return false;
        }

        // method to apply for a loan
        private async Task ApplyForLoanAsync(Client client)
        {
            _console.Clear();

            var headerPanel = new Panel("[bold]Enter loan and income details[/]")
                .Header("[bold cyan]Apply for Loan[/]", Justify.Center)
                .RoundedBorder()
                .BorderColor(Color.Cyan)
                .Padding(1, 1);

            AnsiConsole.Write(headerPanel);

            _console.Write("Loan amount ($): ");
            if (!decimal.TryParse(_console.ReadLine(), out var loanAmount))
            {
                AnsiConsole.MarkupLine("[red]✗ Invalid amount.[/]");
                return;
            }

            if (loanAmount <= 0)
            {
                AnsiConsole.MarkupLine("[red]✗ Loan amount must be greater than 0.[/]");
                return;
            }

            _console.Write("Monthly income ($): ");
            if (!decimal.TryParse(_console.ReadLine(), out var monthlyIncome))
            {
                AnsiConsole.MarkupLine("[red]✗ Invalid income amount.[/]");
                return;
            }

            if (monthlyIncome <= 0)
            {
                AnsiConsole.MarkupLine("[red]✗ Monthly income must be greater than 0.[/]");
                return;
            }

            try
            {
                // Create a new loan request
                var loanRequest = new LoanRequest(client.Id, $"{client.Name} {client.LastName}", loanAmount, monthlyIncome);

                // Add the loan request to the file manager for saving and future admin review
                await _fileManager.AddLoanRequestAsync(loanRequest);

                // Check if loan is safe 
                if (loanRequest.IsLoanSafe())
                {
                    AnsiConsole.MarkupLine($"\n[magenta]✓ Loan request of [bold]${loanAmount:F2}[/] submitted successfully![/]");
                    AnsiConsole.MarkupLine("[yellow]Your loan appears to be within safe lending parameters.[/]");
                }
                else
                {
                    AnsiConsole.MarkupLine($"\n[magenta]✓ Loan request of [bold]${loanAmount:F2}[/] submitted successfully![/]");
                    AnsiConsole.MarkupLine("[yellow]⚠ Your loan may be at higher risk. An admin will review your application.[/]");
                }
                AnsiConsole.MarkupLine("[cyan]Status: [bold]Pending[/][/]");
            }
            catch (Exception ex)
            {
                AnsiConsole.MarkupLine($"[red]✗ Error submitting loan request: {ex.Message}[/]");
            }
        }

        private void DisplayTransactionHistory(Client client)
        {
            try
            {
                _console.Clear();

                var headerPanel = new Panel("[bold]View all your transactions[/]")
                    .Header("[bold cyan]Transaction History[/]", Justify.Center)
                    .RoundedBorder()
                    .BorderColor(Color.Cyan)
                    .Padding(1, 1);

                AnsiConsole.Write(headerPanel);

                if (client.BankAccount.TransactionHistory == null || client.BankAccount.TransactionHistory.Count == 0)
                {
                    AnsiConsole.MarkupLine("[yellow]ℹ No transactions yet.[/]");
                }
                else
                {
                    var table = new Table()
                        .AddColumn("[cyan]Date[/]")
                        .AddColumn("[cyan]Amount[/]")
                        .AddColumn("[cyan]Balance[/]")
                        .Border(TableBorder.Rounded);

                    decimal? previousBalance = null;

                    foreach (var transaction in client.BankAccount.TransactionHistory)
                    {
                        try
                        {
                            var transactionString = transaction.ToString();

                            // Extract date and time 
                            int dateStart = transactionString.IndexOf('[') + 1;
                            int dateEnd = transactionString.IndexOf(']');
                            string date = transactionString.Substring(dateStart, dateEnd - dateStart);

                            // Extract balance
                            int balanceStart = transactionString.LastIndexOf("Balance: $") + "Balance: $".Length;
                            string balanceStr = transactionString.Substring(balanceStart).Trim();

                            if (decimal.TryParse(balanceStr, out decimal currentBalance))
                            {
                                // Calculate the transaction amount
                                decimal transactionAmount = 0;

                                if (previousBalance.HasValue)
                                {
                                    transactionAmount = currentBalance - previousBalance.Value;
                                }

                                // Format with +/- and color
                                string formattedAmount = transactionAmount >= 0
                                    ? $"[magenta]+${transactionAmount:F2}[/]"
                                    : $"[red]-${Math.Abs(transactionAmount):F2}[/]";

                                var escapedDate = Markup.Escape(date.Trim());
                                var escapedBalance = Markup.Escape($"${currentBalance:F2}");

                                table.AddRow(escapedDate, formattedAmount, escapedBalance);
                                previousBalance = currentBalance;
                            }
                        }
                        catch (Exception ex)
                        {
                            AnsiConsole.MarkupLine($"[yellow]Warning: Could not parse transaction: {ex.Message}[/]");
                        }
                    }

                    AnsiConsole.Write(table);
                }

                AnsiConsole.MarkupLine("\n[yellow]Press any key to continue...[/]");
            }
            catch (Exception ex)
            {
                AnsiConsole.MarkupLine($"[red]✗ Error displaying transaction history: {ex.Message}[/]");
                AnsiConsole.MarkupLine("[yellow]Press any key to continue...[/]");
            }

            _console.ReadKey(true);
        }

        // method to deposit funds
        private async Task DepositAsync(Client client)
        {
            _console.Clear();

            var depositPanel = new Panel("[bold]Enter deposit amount[/]")
                .Header("[bold magenta]Deposit Funds[/]", Justify.Center)
                .RoundedBorder()
                .BorderColor(Color.Magenta)
                .Padding(1, 1);

            AnsiConsole.Write(depositPanel);

            _console.Write("Amount to deposit: $");
            if (decimal.TryParse(_console.ReadLine(), out var depositAmount))
            {
                if (depositAmount <= 0)
                {
                    AnsiConsole.MarkupLine("[red]✗ Amount must be greater than 0.[/]");
                    AnsiConsole.MarkupLine("[yellow]Press any key to continue...[/]");
                    _console.ReadKey(true);
                    return;
                }

                try
                {
                    var confirmContent = $"[bold yellow]Deposit Amount:[/] [cyan]${depositAmount:F2}[/]\n\n[bold]Proceed with this deposit?[/]";
                    var confirmPanel = new Panel(confirmContent)
                        .Header("[bold magenta]Confirm Deposit[/]", Justify.Center)
                        .RoundedBorder()
                        .BorderColor(Color.Magenta)
                        .Padding(1, 1);

                    AnsiConsole.Write(confirmPanel);

                    var optionsPanel = new Panel("[bold cyan]1[/] Confirm\n[bold cyan]2[/] Cancel")
                        .RoundedBorder()
                        .BorderColor(Color.Yellow)
                        .Padding(1, 1);
                    AnsiConsole.Write(optionsPanel);

                    _console.Write("Option: ");

                    if (_console.ReadLine() == "1")
                    {
                        client.BankAccount.Deposit(depositAmount);
                        await _fileManager.UpdateUserAsync(client);
                        AnsiConsole.MarkupLine($"\n[magenta]✓ Deposit successful![/]");
                        AnsiConsole.MarkupLine($"[yellow]Deposited: [bold]+${depositAmount:F2}[/][/]");
                        AnsiConsole.MarkupLine($"[cyan]New balance: [bold]${client.BankAccount.Balance:F2}[/][/]");
                    }
                    else
                    {
                        AnsiConsole.MarkupLine("[yellow]✓ Deposit cancelled.[/]");
                    }
                }
                catch (Exception ex)
                {
                    AnsiConsole.MarkupLine($"[red]✗ Error processing deposit: {ex.Message}[/]");
                }
            }
            else
            {
                AnsiConsole.MarkupLine("[red]✗ Invalid amount entered.[/]");
            }

            AnsiConsole.MarkupLine("[yellow]Press any key to continue...[/]");
            _console.ReadKey(true);
        }
        
        // method to withdraw funds
        private async Task WithdrawAsync(Client client)
        {
            _console.Clear();

            var withdrawPanel = new Panel("[bold]Enter withdrawal amount[/]")
                .Header("[bold cyan]Withdraw Funds[/]", Justify.Center)
                .RoundedBorder()
                .BorderColor(Color.Cyan)
                .Padding(1, 1);

            AnsiConsole.Write(withdrawPanel);

            AnsiConsole.MarkupLine($"[yellow]Available balance: [bold]${client.BankAccount.Balance:F2}[/][/]");
            _console.Write("\nAmount to withdraw: $");

            if (decimal.TryParse(_console.ReadLine(), out var withdrawAmount))
            {
                if (withdrawAmount <= 0)
                {
                    AnsiConsole.MarkupLine("[red]✗ Amount must be greater than 0.[/]");
                    AnsiConsole.MarkupLine("[yellow]Press any key to continue...[/]");
                    _console.ReadKey(true);
                    return;
                }

                if (withdrawAmount > client.BankAccount.Balance)
                {
                    AnsiConsole.MarkupLine($"[red]✗ Insufficient funds![/]");
                    AnsiConsole.MarkupLine($"[yellow]Available: [bold]${client.BankAccount.Balance:F2}[/][/]");
                    AnsiConsole.MarkupLine("[yellow]Press any key to continue...[/]");
                    _console.ReadKey(true);
                    return;
                }

                try
                {
                    var confirmContent = $"[bold red]Withdrawal Amount:[/] [cyan]${withdrawAmount:F2}[/]\n[yellow]Current Balance:[/] [cyan]${client.BankAccount.Balance:F2}[/]\n\n[bold]Proceed with this withdrawal?[/]";
                    var confirmPanel = new Panel(confirmContent)
                        .Header("[bold red]Confirm Withdrawal[/]", Justify.Center)
                        .RoundedBorder()
                        .BorderColor(Color.Red)
                        .Padding(1, 1);

                    AnsiConsole.Write(confirmPanel);

                    var optionsPanel = new Panel("[bold cyan]1[/] Confirm\n[bold cyan]2[/] Cancel")
                        .RoundedBorder()
                        .BorderColor(Color.Yellow)
                        .Padding(1, 1);
                    AnsiConsole.Write(optionsPanel);

                    _console.Write("Option: ");

                    if (_console.ReadLine() == "1")
                    {
                        var success = client.BankAccount.Withdraw(withdrawAmount);
                        if (success)
                        {
                            await _fileManager.UpdateUserAsync(client);
                            AnsiConsole.MarkupLine($"\n[magenta]✓ Withdrawal successful![/]");
                            AnsiConsole.MarkupLine($"[yellow]Withdrawn: [bold]-${withdrawAmount:F2}[/][/]");
                            AnsiConsole.MarkupLine($"[cyan]New balance: [bold]${client.BankAccount.Balance:F2}[/][/]");
                        }
                        else
                        {
                            AnsiConsole.MarkupLine("[red]✗ Withdrawal failed.[/]");
                        }
                    }
                    else
                    {
                        AnsiConsole.MarkupLine("[yellow]✓ Withdrawal cancelled.[/]");
                    }
                }
                catch (Exception ex)
                {
                    AnsiConsole.MarkupLine($"[red]✗ Error processing withdrawal: {ex.Message}[/]");
                }
            }
            else
            {
                AnsiConsole.MarkupLine("[red]✗ Invalid amount entered.[/]");
            }

            AnsiConsole.MarkupLine("[yellow]Press any key to continue...[/]");
            _console.ReadKey(true);
        }
    }
}
