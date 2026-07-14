using System;
using Core.Models;

namespace UI
{
    internal class AdminMenu
    {
        private readonly Interfaces.IConsole _console;

        public AdminMenu(Interfaces.IConsole console)
        {
            _console = console;
        }

        // Show menu for any logged-in user. If returns true, request logout.
        public bool Show(User user)
        {
            // Clear the console and display the logged-in user's email
            _console.Clear();
            _console.WriteLine($"Logged in as: {user.Email}");

            // Display the menu options based on the user's role
            user.DisplayMenu();

            _console.Write("Option: ");
            var choice = _console.ReadLine();

            // Basic logout handling same for both admin and client
            if (choice == MenuKeys.Logout)
            {
                return true;
            }

            // other actions will be implemented soon
            _console.WriteLine("Option handling not implemented yet.");
            _console.WriteLine("Press any key to continue...");
            _console.ReadKey(true);
            return false;
        }
    }
}
