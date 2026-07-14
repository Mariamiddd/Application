using System;
using System.Text;
using System.Text.RegularExpressions;
using Application.InterfaceServices;

namespace UI
{
    public class ConsoleUI
    {
        private readonly IAuthService _auth;
        private UserModel? _currentUser; //storing currect user in memory

        public ConsoleUI(IAuthService auth)
        {
            _auth = auth;
        }

        public void ShowMainMenu()
        {
            while (true)
            {
                if (_currentUser != null)
                {
                    ShowUserMenu();
                }
                else
                {
                    ShowGuestMenu();
                }
            }
        }

        private void ShowGuestMenu()
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
                case "1": DoRegister(); break;
                case "2": DoLogin(); break;
                case "0": Environment.Exit(0); break;
                default: Console.WriteLine("Invalid."); Console.ReadKey(); break;
            }
        }

        private void ShowUserMenu()
        {
            Console.Clear();
            Console.WriteLine($"Logged in as: {_currentUser?.Email}");
            Console.WriteLine("1) Logout");
            Console.Write("Option: ");

            var choice = Console.ReadLine();
            if (choice == "1") _currentUser = null;
        }

        private void DoLogin()
        {
            Console.Write("Email: ");
            var email = Console.ReadLine() ?? "";
            Console.Write("Password: ");
            var password = ReadPassword();

            var user = _auth.Login(email, password);
            if (user != null)
            {
                _currentUser = new UserModel(user.Id, user.Email ?? string.Empty, user.Name, user.lastName, user.isVerified);
                Console.WriteLine("Login successful.");
            }
            else
            {
                Console.WriteLine("Invalid credentials.");
            }
            Console.ReadKey();
        }

        private void DoRegister()
        {
            Console.Write("Email: ");
            var email = Console.ReadLine() ?? string.Empty;

            // basic gmail validation: local part allowed chars and domain must be gmail.com
            var gmailPattern = new Regex("^[A-Za-z0-9._%+-]+@gmail\\.com$", RegexOptions.IgnoreCase);
            if (!gmailPattern.IsMatch(email))
            {
                Console.WriteLine("\nInvalid Gmail address. Please enter a valid address ending with @gmail.com and containing only letters, numbers and . _ % + - characters.");
                Console.WriteLine("Press any key to return to menu...");
                Console.ReadKey();
                return;
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
                // map domain user to lightweight UserModel used by UI
                _currentUser = new UserModel(user.Id, user.Email ?? string.Empty, user.Name, user.lastName, user.isVerified);
                Console.WriteLine("\nRegistration successful. You are now logged in.");
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