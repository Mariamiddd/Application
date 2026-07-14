using System;
using Application.InterfaceServices;

namespace UI
{
    internal class AdminMenu
    {
        public bool Show(UserModel user)
        {
            Console.Clear();
            Console.WriteLine($"Logged in as: {user.Email}");
            Console.WriteLine("1) Logout");
            Console.Write("Option: ");

            var choice = Console.ReadLine();
            if (choice == "1") return true; // request logout
            return false;
        }
    }
}
