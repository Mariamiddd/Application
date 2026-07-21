namespace Core.Models
{

    // account class to hold the balance and transaction history of a user
    public class Account
    {
        // balance property to hold the current balance of the account
        public decimal Balance { get; set; }

        // transaction history property to hold the list of transactions made on the account
        public List<Transaction> TransactionHistory { get; set; }

        public Account()
        {
            Balance = 0;
            TransactionHistory = new List<Transaction>();
        }

        public Account(decimal initialBalance = 0)
        {
            Balance = initialBalance;
            TransactionHistory = new List<Transaction>();
        }

        // deposit method to add money to the account
        public void Deposit(decimal amount)
        {
            if (amount <= 0)
            {
                throw new ArgumentException("Deposit amount must be a positive number.");
            }

            Balance += amount;
            // Log the transaction
            TransactionHistory.Add(new Transaction("Deposit", amount, Balance));
        }

        // withdraw method to withdraw money from the account
        public bool Withdraw(decimal amount)
        {
            if (amount <= 0)
            {
                throw new ArgumentException("Withdrawal amount must be a positive number.");
            }

            if (Balance < amount)
            {
                return false; // Balance is insufficient for the withdrawal
            }

            Balance -= amount;
            // Logs the transaction
            TransactionHistory.Add(new Transaction("Withdrawal", amount, Balance));
            return true;
        }
    }
}