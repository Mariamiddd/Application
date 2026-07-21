using Core.Enums;

namespace Core.Models
{
    // Admin class inherits from User class
    public class Admin : User
    {
        public Admin()
        {
            // Admin role is set automatically when created
            Role = Roles.Admin;
        }

        // Override the DisplayMenu method to show admin menu
        public override void DisplayMenu()
        {
            Console.WriteLine("\n--- Admin's Menu ---");
            Console.WriteLine("1. Process Loan Requests (Approve/Reject)");
            Console.WriteLine("2. Delete User Account");
            Console.WriteLine("0. Logout");
        }
    }
}
