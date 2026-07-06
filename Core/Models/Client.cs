using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Models
{
    public class Client : User
    {
        public decimal Balance;

        public override void DisplayMenu()
        {
            Console.WriteLine($"Welcome, {Username}!");
            Console.WriteLine("Client's Menu");
        }

    }
}
