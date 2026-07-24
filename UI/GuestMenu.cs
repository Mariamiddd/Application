using System;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Application.InterfaceServices;
using Core.Enums;
using Core.Constants;
using Spectre.Console;

namespace UI
{
    internal class GuestMenu
    {
        private readonly IAuthService _auth;
        private readonly UI.Interfaces.IConsole _console;

        public GuestMenu(IAuthService auth, UI.Interfaces.IConsole console)
        {
            _auth = auth;
            _console = console;
        }

        // guest menu for unauthenticated users
        public async Task<Core.Models.User?> ShowAsync()
        {
            _console.Clear();

            var menuContent = "[bold cyan]1[/] Register\n[bold cyan]2[/] Login\n[bold cyan]0[/] Exit";
            var panel = new Panel(menuContent)
                .Header("[bold]Guest Menu[/]", Justify.Center)
                .RoundedBorder()
                .BorderColor(Color.Cyan)
                .Padding(1, 1);

            AnsiConsole.Write(panel);
            _console.Write("Option: ");

            var choice = _console.ReadLine();
            switch (choice)
            {
                case "1": return await DoRegisterAsync();
                case "2": return await DoLoginAsync();
                case "0": Environment.Exit(0); return null;
                default:
                    AnsiConsole.MarkupLine("[red]Invalid option.[/]");
                    _console.ReadKey(true);
                    return null;
            }
        }

        private async Task<Core.Models.User?> DoLoginAsync()
        {
            _console.Clear();

            var panel = new Panel("[bold]Enter your credentials[/]")
                .Header("[bold]Login[/]", Justify.Center)
                .RoundedBorder()
                .BorderColor(Color.Green)
                .Padding(1, 1);

            AnsiConsole.Write(panel);

            _console.Write("Email: ");
            var email = _console.ReadLine() ?? "";
            _console.Write("Password: ");
            var password = UI.Helpers.InputHelpers.ReadPassword();

            try
            {
                var user = await _auth.LoginAsync(email, password);
                if (user != null)
                {
                    // Login successful, return the user object
                    AnsiConsole.MarkupLine("[magenta]✓ Login successful.[/]");
                    _console.ReadKey(true);
                    return user; 
                }
                AnsiConsole.MarkupLine("[red]✗ Invalid credentials.[/]");
                _console.ReadKey(true);
                return null;
            }
            catch (ArgumentException ex)
            {
                AnsiConsole.MarkupLine($"[yellow]⚠ Validation error: {ex.Message}[/]");
            }
            catch (Exception ex)
            {
                AnsiConsole.MarkupLine($"[red]✗ Login failed: {ex.Message}[/]");
            }

            AnsiConsole.MarkupLine("[yellow]Press any key to continue...[/]");
            _console.ReadKey(true);
            return null;
        }

        private async Task<Core.Models.User?> DoRegisterAsync()
        {
            // role selection menu
            _console.Clear();

            var roleContent = "[bold cyan]1[/] Register as Client\n[bold cyan]2[/] Register as Admin\n[bold cyan]0[/] Cancel";
            var rolePanel = new Panel(roleContent)
                .Header("[bold]Select Role[/]", Justify.Center)
                .RoundedBorder()
                .BorderColor(Color.Yellow)
                .Padding(1, 1);

            AnsiConsole.Write(rolePanel);
            _console.Write("Option: ");

            var roleChoice = _console.ReadLine();
            Roles selectedRole;

            switch (roleChoice)
            {
                case "1":
                    selectedRole = Roles.User;
                    break;
                case "2":
                    // Admin registration requires a security code
                    if (!ValidateAdminCode())
                    {
                        AnsiConsole.MarkupLine("[red]Admin registration cancelled.[/]");
                        _console.ReadKey(true);
                        return null;
                    }
                    selectedRole = Roles.Admin;
                    break;
                case "0":
                    return null;
                default:
                    AnsiConsole.MarkupLine("[red]Invalid role selection.[/]");
                    _console.ReadKey(true);
                    return null;
            }

            _console.Clear();
            var regPanel = new Panel("[bold]Enter your registration details[/]")
                .Header("[bold]Registration[/]", Justify.Center)
                .RoundedBorder()
                .BorderColor(Color.Cyan)
                .Padding(1, 1);

            AnsiConsole.Write(regPanel);

            _console.Write("Email: ");
            var email = _console.ReadLine() ?? string.Empty;

            var gmailPattern = new Regex("^[A-Za-z0-9._%+-]+@gmail\\.com$", RegexOptions.IgnoreCase);
            if (!gmailPattern.IsMatch(email))
            {
                AnsiConsole.MarkupLine("[red]✗ Invalid Gmail address. Please enter a valid address ending with @gmail.com and containing only letters, numbers and . _ % + - characters.[/]");
                AnsiConsole.MarkupLine("[yellow]Press any key to return to menu[/]");
                _console.ReadKey(true);
                return null;               
            }

            _console.Write("Password: ");
            var password = UI.Helpers.InputHelpers.ReadPassword();

            _console.Write("First name: ");
            var first = _console.ReadLine();

            _console.Write("Last name: ");
            var last = _console.ReadLine();

            try
            {
                // Attempt to register the user
                var user = await _auth.RegisterAsync(email, password, first ?? string.Empty, last ?? string.Empty, selectedRole);
                string roleString = selectedRole == Roles.Admin ? "Admin" : "Client";
                AnsiConsole.MarkupLine($"[magenta]✓ Registration successful as {roleString}. You are now logged in.[/]");
                _console.ReadKey(true);
                return user;
            }
            catch (ArgumentException ex)
            {
                AnsiConsole.MarkupLine($"[yellow]⚠ Validation error: {ex.Message}[/]");
            }
            catch (InvalidOperationException ex)
            {
                AnsiConsole.MarkupLine($"[red]✗ Registration failed: {ex.Message}[/]");
            }
            catch (Exception ex)
            {
                AnsiConsole.MarkupLine($"[red]✗ Unexpected error: {ex.Message}[/]");
            }

            AnsiConsole.MarkupLine("[yellow]Press any key to continue...[/]");
            _console.ReadKey(true);
            return null;
        }

        private bool ValidateAdminCode()
        {
            _console.Clear();

            var codePanel = new Panel("[bold]Registering as admin requires special code![/]\n\nEnter the admin registration code:")
                .Header("[bold yellow]⚠ Admin Registration Code Required[/]", Justify.Center)
                .RoundedBorder()
                .BorderColor(Color.Yellow)
                .Padding(1, 1);

            AnsiConsole.Write(codePanel);
            _console.Write("Code: ");

            string? enteredCode = _console.ReadLine();

            // check if admin registration code is correct
            if (string.IsNullOrEmpty(enteredCode) || enteredCode != SecurityConstants.ADMIN_REGISTRATION_CODE)
            {
                AnsiConsole.MarkupLine("[red]✗ Incorrect admin registration code![/]");
                AnsiConsole.MarkupLine("[yellow]Try again![/]");
                _console.ReadKey(true);
                return false;
            }

            AnsiConsole.MarkupLine("[magenta]✓ Code verified! Proceeding with admin registration.[/]");
            _console.ReadKey(true);

            // Additional confirmation key is yes
            _console.Clear();

            var confirmPanel = new Panel("[bold]Are you sure you want to create an admin account?[/]")
                .Header("[bold]Confirm Admin Registration[/]", Justify.Center)
                .RoundedBorder()
                .BorderColor(Color.Yellow)
                .Padding(1, 1);

            AnsiConsole.Write(confirmPanel);
            AnsiConsole.MarkupLine("Type [bold yellow]YES[/] to confirm admin registration: ");
            _console.Write("Input: ");

            string? adminConfirmation = _console.ReadLine()?.Trim().ToUpper();

            if (adminConfirmation != "YES")
            {
                AnsiConsole.MarkupLine("[red]✗ Admin registration cancelled.[/]");
                _console.ReadKey(true);
                return false;
            }

            AnsiConsole.MarkupLine("[magenta]✓ Admin account confirmed![/]");
            _console.ReadKey(true);
            return true;
        }


    }
}
