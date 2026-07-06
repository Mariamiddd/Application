using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Models
{
    public class Admin : User
    {
        public override void DisplayMenu()
        {

            Console.WriteLine($"Welcome, {Username}!");
            Console.WriteLine("Admin's Menu");
        }
    }
}
