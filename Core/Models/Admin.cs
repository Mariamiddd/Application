using Core.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Models
{

    // Admin inherits from User and has additional functionalities specific to admin users
    public class Admin : User
        {
            public Admin()
            {
            // from the moment admin is created, we set the role to Admin
            Role = Roles.Admin;
            }
        public void ProcessLoan(LoanRequest request, Client client)
        {
            if (request.IsLoanSafe())
            {
                request.Status = LoanStatus.Approved;
                client.BankAccount.Deposit(request.Amount);
                Console.WriteLine($"Loan approved for client {client.Name}. Amount: {request.Amount}");
            }

            else
            {
                request.Status = LoanStatus.Rejected;
                Console.WriteLine($"Loan rejected for client {client.Name}. Amount: {request.Amount}");
            }
        }

        // polimorphic method to display admin menu
        public override void DisplayMenu()
        {
            Console.WriteLine("\n--- Admin's Menu ---");
            Console.WriteLine("1. Process Loan Requests (Approve/Reject)");
            Console.WriteLine("0. Logout");
        }



    }
}
