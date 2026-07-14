using System;

namespace Core.Models
{
    public class Account
    {
        // Balance has a public setter so serializers can set the value during deserialization
        public decimal Balance { get; set; }

        public Account(decimal initialBalance = 0)
        {
            Balance = initialBalance;
        }

        // method to deposit money into the account
        public void Deposit(decimal amount)
        {
            if (amount <= 0)
            {
                throw new ArgumentException("Deposit amount must be a positive number.");
            }

            Balance += amount;
        }

        // method to withdraw money from the account
        public bool Withdraw(decimal amount)
        {
            if (amount <= 0)
            {
                throw new ArgumentException("Withdrawal amount must be a positive number.");
            }

            if (Balance < amount)
            {
                return false; // balance is insufficient for the withdrawal
            }

            Balance -= amount;
            return true;
        }
    }
}