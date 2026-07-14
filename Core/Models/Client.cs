using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Models
{

        public class Client : User
        {
        // Client class has bank account property
        public Account BankAccount { get; set; }

            public Client()
            {
            // initialize bank account
            BankAccount = new Account();

            // role is set to User by default
            Role = Enums.Roles.User;
            }

        public override void DisplayMenu()
        {
           Console.WriteLine("\n--- Client's Menu ---");
            Console.WriteLine("1. Check Balance");
            Console.WriteLine("2. Deposit Funds");
            Console.WriteLine("3. Withdraw Funds");
            Console.WriteLine("4. Apply for Loan");
            Console.WriteLine("0. Logout");
            
        } 

        }
}
