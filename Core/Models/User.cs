using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Models
{
    public class User
    {
        public string Username = "";
        public string Role = "";
    
    public virtual void DisplayMenu()
        {
            Console.WriteLine("MENU");
        }
    }
}