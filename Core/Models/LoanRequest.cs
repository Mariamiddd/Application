using Core.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Models
{

        public class LoanRequest
        {
            public int ClientId { get; set; }
            public decimal Amount { get; set; }
            public decimal Income { get; set; }
            public LoanStatus Status { get; set; }

        // constructor to initialize a new loan request
        public LoanRequest(int clientId, decimal amount, decimal income)
            {
                ClientId = clientId;
                Amount = amount;
                Income = income;
                Status = LoanStatus.Pending; // default status when a new loan request is created
        }

        public bool IsLoanSafe()
        {
            // 1. constants for living wage and maximum loan term/ can be uupdated later!!
            const decimal livingWage = 300m; // minimum monthly income required for basic living expenses
            const int maxMonths = 36;        // maximum loan term in months

            // 2. calculate the monthly payment for the loan/ simple logic can be improved later too!
            decimal monthlyPayment = Amount / maxMonths;

            // 3. calculate the remaining income after deducting the living wage
            decimal remainingIncome = Income - livingWage;

            // 4. if remaining income is greater than or equal to the monthly payment, the loan is considered safe
            return remainingIncome >= monthlyPayment;
        }
    }
}
