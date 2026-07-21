using Core.Enums;

namespace Core.Models
{

        public class LoanRequest
        {
            public int Id { get; set; }
            public int ClientId { get; set; }
            public string ClientName { get; set; } = string.Empty;
            public decimal Amount { get; set; }
            public decimal Income { get; set; }
            public LoanStatus Status { get; set; }
            public DateTime RequestDate { get; set; }

        // default constructor to initialize a new loan request with default values
        public LoanRequest()
        {
            Status = LoanStatus.Pending;
            RequestDate = DateTime.Now;
        }

        //  constructor to initialize a new loan request with specified values for clientId, clientName, amount, and income
        public LoanRequest(int clientId, string clientName, decimal amount, decimal income)
            {
                ClientId = clientId;
                ClientName = clientName;
                Amount = amount;
                Income = income;
                Status = LoanStatus.Pending; 
                RequestDate = DateTime.Now;
        }

        public bool IsLoanSafe()
        {
            // 1. constants for living wage and maximum loan term
            const decimal livingWage = 300m; // minimum monthly income required for basic living expenses
            const int maxMonths = 36;        // maximum loan term in months

            // 2. calculate the monthly payment for the loan 
            decimal monthlyPayment = Amount / maxMonths;

            // 3. calculate the remaining income after deducting the living wage
            decimal remainingIncome = Income - livingWage;

            // 4. if remaining income is greater than or equal to the monthly payment, the loan is considered safe
            return remainingIncome >= monthlyPayment;
        }
    }
}
