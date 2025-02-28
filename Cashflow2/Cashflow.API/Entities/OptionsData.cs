namespace Cashflow.API.Entities;

public class PlayerOptions
{
    public List<Profession> Professions { get; set; } = new();

    public PlayerOptions()
    {
        Professions =
        [
            new Profession
            {
                Name = "Software Engineer", Salary = 3100, ChildExpense = 170, OtherExpenses = 710, Savings = 500,
                Liabilities = [
                    new Liability { Name = "Car", Amount = 5000, InterestRate = 0.05M, Term = FinancialConstants.CAR_LOAN_TERM },
                    new Liability { Name = "Mortgage", Amount = 47000, InterestRate = 0.06M, Term = FinancialConstants.MORTGAGE_TERM },
                    new Liability { Name = "Student Loans", Amount = 6000, InterestRate = 0.05M, Term = FinancialConstants.STUDENT_LOANS_TERM },
                    new Liability { Name = "Credit Card", Amount = 4000, InterestRate = 0.18M, Term = FinancialConstants.CREDIT_CARD_TERM },
                ]
            },
            new Profession { Name = "Software Developer" },
            new Profession { Name = "Software Architect" },
            new Profession { Name = "Software Tester" },
            new Profession { Name = "Software Analyst" },
            new Profession { Name = "Software Manager" },
            new Profession { Name = "Software Consultant" }
        ];
    }
}