using System;
using System.Text;
using System.Text.RegularExpressions;
using Application.InterfaceServices;

namespace UI
{
    internal class GuestMenu
    {
        private readonly IAuthService _auth;

        public GuestMenu(IAuthService auth)
        {
            _auth = auth;
        }

        // Show guest menu, return logged in user or null
        public UserModel? Show()
        {
            Console.Clear();
            Console.WriteLine("=== Guest Menu ===");
            Console.WriteLine("1) Register");
            Console.WriteLine("2) Login");
            Console.WriteLine("0) Exit");
            Console.Write("Option: ");

            var choice = Console.ReadLine();
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

        private UserModel? DoLogin()
        {
            Console.Write("Email: ");
            var email = Console.ReadLine() ?? "";
            Console.Write("Password: ");
            var password = ReadPassword();

            var user = _auth.Login(email, password);
            if (user != null)
            {
                Console.WriteLine("Login successful.");
                Console.ReadKey();
                return new UserModel(user.Id, user.Email ?? string.Empty, user.Name, user.lastName, user.isVerified);
            }

            Console.WriteLine("Invalid credentials.");
            Console.ReadKey();
            return null;
        }

        private UserModel? DoRegister()
        {
            Console.Write("Email: ");
            var email = Console.ReadLine() ?? string.Empty;

            var gmailPattern = new Regex("^[A-Za-z0-9._%+-]+@gmail\\.com$", RegexOptions.IgnoreCase);
            if (!gmailPattern.IsMatch(email))
            {
                Console.WriteLine("\nInvalid Gmail address. Please enter a valid address ending with @gmail.com and containing only letters, numbers and . _ % + - characters.");
                Console.WriteLine("Press any key to return to menu...");
                Console.ReadKey();
                return null;
            }

            Console.Write("Password: ");
            var password = ReadPassword();

            Console.Write("First name (optional): ");
            var first = Console.ReadLine();

            Console.Write("Last name (optional): ");
            var last = Console.ReadLine();

            try
            {
                var user = _auth.Register(email, password, first ?? string.Empty, last ?? string.Empty);
                Console.WriteLine("\nRegistration successful. You are now logged in.");
                Console.ReadKey();
                return new UserModel(user.Id, user.Email ?? string.Empty, user.Name, user.lastName, user.isVerified);
            }
            catch (ArgumentException ex)
            {
                Console.WriteLine($"\nValidation error: {ex.Message}");
            }
            catch (InvalidOperationException ex)
            {
                Console.WriteLine($"\nRegistration failed: {ex.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\nUnexpected error: {ex.Message}");
            }

            Console.WriteLine("\nPress any key to continue...");
            Console.ReadKey();
            return null;
        }

        private static string ReadPassword()
        {
            var sb = new StringBuilder();
            ConsoleKeyInfo key;
            while ((key = Console.ReadKey(true)).Key != ConsoleKey.Enter)
            {
                if (key.Key == ConsoleKey.Backspace && sb.Length > 0)
                {
                    sb.Length--;
                    Console.Write("\b \b");
                }
                else if (!char.IsControl(key.KeyChar))
                {
                    sb.Append(key.KeyChar);
                    Console.Write('*');
                }
            }
            Console.WriteLine();
            return sb.ToString();
        }
    }
}
