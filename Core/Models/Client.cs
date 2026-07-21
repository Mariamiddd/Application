namespace Core.Models
{
    // client class inherits from user class 
    public class Client : User
    {
        // Each client has a bank account when created 
        public Account BankAccount { get; set; } = new Account();

        public Client()
        {
            // this sets the role of the user to client when created
            Role = Enums.Roles.User;
        }

        // Override the DisplayMenu method to show client menu (polymorphism)
        public override void DisplayMenu()
        {
            Console.WriteLine("\n--- Client's Menu ---");
            Console.WriteLine("1. Check Balance");
            Console.WriteLine("2. Deposit Funds");
            Console.WriteLine("3. Withdraw Funds");
            Console.WriteLine("4. Apply for Loan");
            Console.WriteLine("5. View Transaction History");
            Console.WriteLine("0. Logout");
        }
    }
}
