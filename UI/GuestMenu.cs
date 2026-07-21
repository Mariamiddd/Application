using System;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Application.InterfaceServices;
using Core.Enums;
using Core.Constants;

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
            _console.WriteLine("=== Guest Menu ===");
            _console.WriteLine("1) Register");
            _console.WriteLine("2) Login");
            _console.WriteLine("0) Exit");
            _console.Write("Option: ");

            var choice = _console.ReadLine();
            switch (choice)
            {
                case "1": return await DoRegisterAsync();
                case "2": return await DoLoginAsync();
                case "0": Environment.Exit(0); return null;
                default:
                    Console.WriteLine("Invalid.");
                    Console.ReadKey();
                    return null;
            }
        }

        private async Task<Core.Models.User?> DoLoginAsync()
        {
            Console.Write("Email: ");
            var email = Console.ReadLine() ?? "";
            Console.Write("Password: ");
            var password = UI.Helpers.InputHelpers.ReadPassword();

            var user = await _auth.LoginAsync(email, password);
            if (user != null)
            {
                // Login successful, return the user object
                _console.WriteLine("Login successful.");
                _console.ReadKey(true);
                return user; 
            }
            _console.WriteLine("Invalid credentials.");
            _console.ReadKey(true);
            return null;
        }

        private async Task<Core.Models.User?> DoRegisterAsync()
        {
            // role selection menu
            _console.Clear();
            _console.WriteLine("=== Select Role ===");
            _console.WriteLine("1) Register as Client");
            _console.WriteLine("2) Register as Admin");
            _console.WriteLine("0) Cancel");
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
                        _console.WriteLine("Admin registration cancelled.");
                        _console.ReadKey(true);
                        return null;
                    }
                    selectedRole = Roles.Admin;
                    break;
                case "0":
                    return null;
                default:
                    Console.WriteLine("Invalid role selection.");
                    Console.ReadKey();
                    return null;
            }

            _console.Write("Email: ");
            var email = _console.ReadLine() ?? string.Empty;

            var gmailPattern = new Regex("^[A-Za-z0-9._%+-]+@gmail\\.com$", RegexOptions.IgnoreCase);
            if (!gmailPattern.IsMatch(email))
            {
                _console.WriteLine("Invalid Gmail address. Please enter a valid address ending with @gmail.com and containing only letters, numbers and . _ % + - characters.");
                _console.WriteLine("Press any key to return to menu");
                _console.ReadKey(true);
                return null;               

            }

            Console.Write("Password: ");
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
                _console.WriteLine($"Registration successful as {roleString}. You are now logged in.");
                _console.ReadKey(true);
                return user;
            }
            catch (ArgumentException ex)
            {
                _console.WriteLine($"\nValidation error: {ex.Message}");
            }
            catch (InvalidOperationException ex)
            {
                _console.WriteLine($"\nRegistration failed: {ex.Message}");
            }
            catch (Exception ex)
            {
                _console.WriteLine($"\nUnexpected error: {ex.Message}");
            }

            _console.WriteLine("\nPress any key to continue...");
            _console.ReadKey(true);
            return null;
        }

        private bool ValidateAdminCode()
        {
            _console.Clear();
            _console.WriteLine("=== Admin Registration Code Required ===\n");
            _console.WriteLine("Registering as admin requires special code!");
            _console.WriteLine("Enter the admin registration code:");
            _console.Write("Code: ");

            string? enteredCode = _console.ReadLine();

            // check if admin registration code is correct
            if (string.IsNullOrEmpty(enteredCode) || enteredCode != SecurityConstants.ADMIN_REGISTRATION_CODE)
            {
                _console.WriteLine("Incorrect admin registration code!");
                _console.WriteLine("Try again!");
                _console.ReadKey(true);
                return false;
            }

            _console.WriteLine("Code verified! Proceeding with admin registration.");
            _console.ReadKey(true);

            // Additional confirmation key is yes
            _console.Clear();
            _console.WriteLine("Are you sure you want to create an admin account?");
            _console.Write("Type 'YES' to confirm admin registration: ");

            string? adminConfirmation = _console.ReadLine()?.Trim().ToUpper();

            if (adminConfirmation != "YES")
            {
                _console.WriteLine("Admin registration cancelled.");
                _console.ReadKey(true);
                return false;
            }

            _console.WriteLine("Admin account confirmed!");
            _console.ReadKey(true);
            return true;
        }


    }
}
