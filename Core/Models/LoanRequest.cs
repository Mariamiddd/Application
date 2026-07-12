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

            // კონსტრუქტორი
            public LoanRequest(int clientId, decimal amount, decimal income)
            {
                ClientId = clientId;
                Amount = amount;
                Income = income;
                Status = LoanStatus.Pending; // თავდაპირველად ყოველთვის მოლოდინშია
            }

        public bool IsLoanSafe()
        {
            // 1. მუდმივები (კონსტანტები) - შეგვიძლია მოგვიანებით შევცვალოთ
            const decimal livingWage = 300m; // საარსებო მინიმუმი 
            const int maxMonths = 36;        // სესხის მაქსიმალური ვადა

            // 2. ყოველთვიური გადასახადი (პროცენტის გარეშე, მარტივი ლოგიკით)
            decimal monthlyPayment = Amount / maxMonths;

            // 3. ხელზე დარჩენილი თანხა საარსებო მინიმუმის გამოკლების შემდეგ
            decimal remainingIncome = Income - livingWage;

            // 4. ვაბრუნებთ შედეგს: ფარავს თუ არა დარჩენილი თანხა ყოველთვიურ შენატანს?
            return remainingIncome >= monthlyPayment;
        }
    }
}
