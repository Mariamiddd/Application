using Core.Enums;
using Core.Interfaces;
using Core.Models;
using Spectre.Console;
using System;
using System.Threading.Tasks;

namespace UI
{
    // AdminMenu class provides the interface for admin users to manage loan requests and user accounts.
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
            // Clear the console for a fresh display
            _console.Clear();

            var headerPanel = new Panel($"[bold cyan]Logged in as: {user.Email}[/]")
                .Header("[bold yellow]Admin Dashboard[/]", Justify.Center)
                .RoundedBorder()
                .BorderColor(Color.Yellow)
                .Padding(1, 1);

            AnsiConsole.Write(headerPanel);

            // Display styled menu options
            var menuContent = "[bold yellow]1[/] Process Loan Requests (Approve/Reject)\n[bold yellow]2[/] Delete User Account\n[bold yellow]0[/] Logout";
            var menuPanel = new Panel(menuContent)
                .Header("[bold yellow]Admin Options[/]", Justify.Center)
                .RoundedBorder()
                .BorderColor(Color.Yellow)
                .Padding(1, 1);
            AnsiConsole.Write(menuPanel);

            _console.Write("Option: ");
            var choice = _console.ReadLine();

            //logout handling same for both admin and client
            if (choice == "0")
            {
                return true;
            }

            // handling admin options
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
                AnsiConsole.MarkupLineInterpolated($"[red]✗ User type not supported for admin actions.[/]");
            }

            AnsiConsole.MarkupLine("[yellow]Press any key to continue...[/]");
            _console.ReadKey(true);
            return false;
        }

        private async Task ProcessLoanRequestsAsync(Admin admin)
        {
            _console.Clear();

            var headerPanel = new Panel("[bold]Process Loan Requests[/]")
                .Header("[bold cyan]Pending Loans[/]", Justify.Center)
                .RoundedBorder()
                .BorderColor(Color.Cyan)
                .Padding(1, 1);

            AnsiConsole.Write(headerPanel);
            AnsiConsole.Write(new Rule("[cyan]Pending Loan Requests[/]").RuleStyle("cyan"));

            // Get all pending loan requests
            try
            {
                var pendingLoans = await _fileManager.GetPendingLoanRequestsAsync();

                if (pendingLoans.Count == 0)
                {
                    AnsiConsole.MarkupLine("[yellow] !No pending loan requests at this time.[/]");
                    AnsiConsole.MarkupLine("[yellow]Press any key to continue...[/]");
                    _console.ReadKey(true);
                    return;
                }

                AnsiConsole.MarkupLine($"[cyan]Found [bold]{pendingLoans.Count}[/] pending loan request(s):[/]\n");

                // Display all pending loans
                for (int i = 0; i < pendingLoans.Count; i++)
                {
                    var loan = pendingLoans[i];
                    AnsiConsole.MarkupLine($"[bold]{i + 1}.[/] Client: {loan.ClientName} | Amount: [yellow]${loan.Amount}[/] | Income: [magenta]${loan.Income}[/]");
                    AnsiConsole.MarkupLine($"   Status: [cyan]{loan.Status}[/] | Submitted: {loan.RequestDate:MM/dd/yyyy}");
                }

                AnsiConsole.MarkupLine($"\n[cyan]Select loan to process (1-{pendingLoans.Count}) or 0 to cancel: [/]");
                _console.Write("Option: ");

                if (!int.TryParse(_console.ReadLine(), out int choice) || choice < 1 || choice > pendingLoans.Count)
                {
                    AnsiConsole.MarkupLine("[red]✗ Invalid selection.[/]");
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
                AnsiConsole.MarkupLine($"[red]✗ Error processing loan requests: {ex.Message}[/]");
            }
        }

        private async Task ApproveLoanDialogAsync(LoanRequest loan)
        {
            _console.Clear();

            var reviewPanel = new Panel($"[bold]Client: {loan.ClientName}[/]\nAmount: [yellow]${loan.Amount}[/]\nIncome: [magenta]${loan.Income}[/]\nDate: {loan.RequestDate:MM/dd/yyyy HH:mm}")
                .Header("[bold cyan]Loan Review[/]", Justify.Center)
                .RoundedBorder()
                .BorderColor(Color.Cyan)
                .Padding(1, 1);

            AnsiConsole.Write(reviewPanel);
            AnsiConsole.Write(new Rule("[cyan]Loan Assessment[/]").RuleStyle("cyan"));

            // Check if loan is safe
            bool isSafe = loan.IsLoanSafe();
            var safetyStatus = isSafe ? "[magenta]✓ SAFE[/]" : "[red]✗ RISKY[/]";
            AnsiConsole.MarkupLine($"Loan Safety: {safetyStatus}");
            AnsiConsole.MarkupLine($"Status: [cyan]{loan.Status}[/]\n");

            var optionsPanel = new Panel("[bold cyan]1[/] Approve Loan\n[bold cyan]2[/] Reject Loan\n[bold cyan]0[/] Cancel")
                .RoundedBorder()
                .BorderColor(Color.Magenta)
                .Padding(1, 1);

            AnsiConsole.Write(optionsPanel);
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
                    AnsiConsole.MarkupLine("[red]✗ Invalid selection.[/]");
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

                AnsiConsole.MarkupLine($"\n[magenta]✓ Loan of [bold]${loan.Amount}[/] APPROVED[/] for {loan.ClientName}");
                AnsiConsole.MarkupLine("[yellow]Amount added to client's balance.[/]");
            }
            catch (Exception ex)
            {
                AnsiConsole.MarkupLine($"[red]✗ Error approving loan: {ex.Message}[/]");
            }
        }

        private async Task RejectLoanAsync(LoanRequest loan)
        {
            try
            {
                // Update loan status
                loan.Status = LoanStatus.Rejected;
                await _fileManager.UpdateLoanRequestAsync(loan);

                AnsiConsole.MarkupLine($"\n[red]✗ Loan of [bold]${loan.Amount}[/] REJECTED[/] for {loan.ClientName}");
                AnsiConsole.MarkupLine("[yellow]Client has been notified.[/]");
            }
            catch (Exception ex)
            {
                AnsiConsole.MarkupLine($"[red]✗ Error rejecting loan: {ex.Message}[/]");
            }
        }

        private async Task DeleteUserAccountAsync()
        {
            _console.Clear();

            var headerPanel = new Panel("[bold]Enter user email to delete[/]")
                .Header("[bold red]Delete User Account[/]", Justify.Center)
                .RoundedBorder()
                .BorderColor(Color.Red)
                .Padding(1, 1);

            AnsiConsole.Write(headerPanel);
            AnsiConsole.Write(new Rule("[red]Confirm User Deletion[/]").RuleStyle("red"));

            _console.Write("Email: ");
            string userEmail = (_console.ReadLine() ?? string.Empty).Trim();

            if (string.IsNullOrEmpty(userEmail))
            {
                AnsiConsole.MarkupLine("[red]✗ Invalid email. Operation cancelled.[/]");
                AnsiConsole.MarkupLine("[yellow]Press any key to continue...[/]");
                _console.ReadKey(true);
                return;
            }

            try
            {
                // Retrieve user by email
                var userToDelete = await _fileManager.GetUserByEmailAsync(userEmail);

                if (userToDelete == null)
                {
                    AnsiConsole.MarkupLine($"[red]✗ User with email '[bold]{userEmail}[/]' not found.[/]");
                    AnsiConsole.MarkupLine("[yellow]Press any key to continue...[/]");
                    _console.ReadKey(true);
                    return;
                }

                // Warning for admin deletion
                if (userToDelete.Role == Roles.Admin)
                {
                    var adminWarning = new Panel("[bold red]⚠ WARNING: Admin Account[/]\nYou are about to delete another ADMIN account!")
                        .BorderColor(Color.Red)
                        .RoundedBorder()
                        .Padding(1, 1);
                    AnsiConsole.Write(adminWarning);
                }

                // Confirm deletion by re-entering email
                var confirmPanel = new Panel($"[bold red]⚠ This action CANNOT be undone![/]\n\nUser Email: [yellow]{userToDelete.Email}[/]\nUser Role: [yellow]{userToDelete.Role}[/]")
                    .Header("[bold]Confirm Deletion[/]", Justify.Center)
                    .BorderColor(Color.Red)
                    .RoundedBorder()
                    .Padding(1, 1);
                AnsiConsole.Write(confirmPanel);

                _console.Write("Please re-enter the email to confirm deletion: ");
                string confirmationEmail = (_console.ReadLine() ?? string.Empty).Trim();

                if (confirmationEmail != userEmail)
                {
                    AnsiConsole.MarkupLine("\n[magenta]✓ Email confirmation mismatch. Deletion cancelled.[/]");
                    AnsiConsole.MarkupLine("[yellow]Press any key to continue...[/]");
                    _console.ReadKey(true);
                    return;
                }

                // Perform deletion
                await _fileManager.DeleteUserAsync(userToDelete.Id);

                AnsiConsole.MarkupLine($"\n[magenta]✓ User account '[bold]{userEmail}[/]' has been successfully deleted.[/]");
            }
            catch (Exception ex)
            {
                AnsiConsole.MarkupLine($"[red]✗ Error deleting user: {ex.Message}[/]");
            }

            AnsiConsole.MarkupLine("[yellow]Press any key to continue...[/]");
            _console.ReadKey(true);
        }
    }
}
