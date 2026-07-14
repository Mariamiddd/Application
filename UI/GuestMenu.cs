using System;
using System.Text.RegularExpressions;
using Application.InterfaceServices;

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

        // Show guest menu, return logged in domain User or null
        public Core.Models.User? Show()
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
                case "1": return DoRegister();
                case "2": return DoLogin();
                case "0": Environment.Exit(0); return null;
                default:
                    Console.WriteLine("Invalid.");
                    Console.ReadKey();
                    return null;
            }
        }

        private Core.Models.User? DoLogin()
        {
            Console.Write("Email: ");
            var email = Console.ReadLine() ?? "";
            Console.Write("Password: ");
            var password = UI.Helpers.InputHelpers.ReadPassword();

            var user = _auth.Login(email, password);
            if (user != null)
            {
                _console.WriteLine("Login successful.");
                _console.ReadKey(true);
                return user; // return domain user for polymorphic menus
            }
            _console.WriteLine("Invalid credentials.");
            _console.ReadKey(true);
            return null;
        }

        private Core.Models.User? DoRegister()
        {
            _console.Write("Email: ");
            var email = _console.ReadLine() ?? string.Empty;

            var gmailPattern = new Regex("^[A-Za-z0-9._%+-]+@gmail\\.com$", RegexOptions.IgnoreCase);
            if (!gmailPattern.IsMatch(email))
            {
                Console.WriteLine("\nInvalid Gmail address. Please enter a valid address ending with @gmail.com and containing only letters, numbers and . _ % + - characters.");
                Console.WriteLine("Press any key to return to menu...");
                Console.ReadKey();
                return null;
            }

            Console.Write("Password: ");
            var password = UI.Helpers.InputHelpers.ReadPassword();

            _console.Write("First name (optional): ");
            var first = _console.ReadLine();

            _console.Write("Last name (optional): ");
            var last = _console.ReadLine();

            try
            {
                var user = _auth.Register(email, password, first ?? string.Empty, last ?? string.Empty);
                _console.WriteLine("\nRegistration successful. You are now logged in.");
                _console.ReadKey(true);
                return user; // return domain user
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


    }
}
