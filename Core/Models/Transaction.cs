namespace Core.Models
{
    // transaction class to represent a bank transaction such as deposit or withdrawal
    public class Transaction
    {
        // type of transaction depositing or withdrawing
        public string Type { get; set; }

        // how much money was deposited or withdrawn
        public decimal Amount { get; set; }

        // time of action/ transaction
        public DateTime Date { get; set; }

        // balance after the transaction was made
        public decimal BalanceAfter { get; set; }

        // constructor to create a new transaction with default values
        public Transaction()
        {
            Type = "Unknown";
            Amount = 0;
            Date = DateTime.Now;
            BalanceAfter = 0;
        }

        // constructor to create a new transaction with specified values for type, amount, and balance after the transaction
        public Transaction(string type, decimal amount, decimal balanceAfter)
        {
            Type = type;
            Amount = amount;
            Date = DateTime.Now;
            BalanceAfter = balanceAfter;
        }

        // override the ToString method to provide a string representation of the transaction (polymorphism)
        public override string ToString()
        {
            return $"[{Date:MM/dd/yyyy HH:mm:ss}] {Type:,-10} ${Amount:F2:,-12} | Balance: ${BalanceAfter:F2}";
        }
    }
}
