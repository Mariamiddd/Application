using Core.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Models
{
    public class User
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string lasName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string VerificationCode { get; set; }
        public bool isVerified { get; set; }
        public Roles Role { get; set; }

        public virtual void DisplayMenu()
        {
            Console.WriteLine("\n--- Client's Menu ---");
            Console.WriteLine("1. Check Balance");
            Console.WriteLine("2. Deposit Funds");
            Console.WriteLine("3. Withdraw Funds");
            Console.WriteLine("4. Apply for Loan");
            Console.WriteLine("5. Logout");

        }
    }
}