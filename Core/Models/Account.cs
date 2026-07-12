using System;

namespace Core.Models
{
    public class Account
    {
        // ბალანსი დაცულია: წაკითხვა (get) საჯაროა, ჩაწერა (set) - დახურული
        public decimal Balance { get; private set; }

        public Account(decimal initialBalance = 0)
        {
            Balance = initialBalance;
        }

        // თანხის შეტანის მეთოდი
        public void Deposit(decimal amount)
        {
            if (amount <= 0)
            {
                throw new ArgumentException("შესატანი თანხა უნდა იყოს დადებითი რიცხვი.");
            }

            Balance += amount;
        }

        // თანხის გატანის მეთოდი
        public bool Withdraw(decimal amount)
        {
            if (amount <= 0)
            {
                throw new ArgumentException("გასატანი თანხა უნდა იყოს დადებითი რიცხვი.");
            }

            if (Balance < amount)
            {
                return false; // ანგარიშზე არ არის საკმარისი თანხა
            }

            Balance -= amount;
            return true;
        }
    }
}