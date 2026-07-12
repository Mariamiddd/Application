using System;
using System.Text;
using Application.InterfaceServices;

namespace UI
{
    public class ConsoleUI
    {
        private readonly IAuthService _auth;

        public ConsoleUI(IAuthService auth) => _auth = auth;
        public void ShowMainMenu()
        {
            while (true)
            {
                Console.Clear();
                Console.WriteLine("Main menu");
                Console.WriteLine("1) Register");
                Console.WriteLine("2) Login");
                Console.WriteLine("0) Exit");
                Console.Write("Choose an option: ");
                var choice = Console.ReadLine() ?? string.Empty;

                switch (choice.Trim())
                {
                    case "1":
                        DoRegister();
                        break;
                    case "2":
                        DoLogin();
                        break;
                    case "0":
                        return;
                    default:
                        Console.WriteLine("Invalid option. Press any key to continue...");
                        Console.ReadKey();
                        break;
                }
            }
        }

        private void DoRegister()
        {
            Console.Clear();
            Console.WriteLine("Register a new user");
            Console.Write("Email: ");
            var email = Console.ReadLine() ?? string.Empty;

            Console.Write("Password: ");
            var password = ReadPassword();

            Console.Write("First name (optional): ");
            var first = Console.ReadLine();

            Console.Write("Last name (optional): ");
            var last = Console.ReadLine();

            try
            {
                var user = _auth.Register(email, password, first ?? string.Empty, last ?? string.Empty);
                Console.WriteLine($"\nUser registered. Id: {user.Id}, Email: {user.Email}");
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

            Console.WriteLine("\nPress any key to return to menu...");
            Console.ReadKey();
        }

        private void DoLogin()
        {
            Console.Clear();
            Console.WriteLine("Login");
            Console.Write("Email: ");
            var email = Console.ReadLine() ?? string.Empty;
            Console.Write("Password: ");
            var password = ReadPassword();

            try
            {
                var user = _auth.Login(email, password);
                if (user is null)
                {
                    Console.WriteLine("\nLogin failed: invalid credentials.");
                }
                else
                {
                    Console.WriteLine($"\nLogin successful. Id: {user.Id}, Email: {user.Email}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\nUnexpected error: {ex.Message}");
            }

            Console.WriteLine("\nPress any key to return to menu...");
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
