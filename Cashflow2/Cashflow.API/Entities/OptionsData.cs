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
            new Profession
            {
                Name = "Software Developer", Salary = 2800, ChildExpense = 150, OtherExpenses = 650, Savings = 400,
                Liabilities = [
                    new Liability { Name = "Car", Amount = 4000, InterestRate = 0.05M, Term = FinancialConstants.CAR_LOAN_TERM },
                    new Liability { Name = "Mortgage", Amount = 38000, InterestRate = 0.06M, Term = FinancialConstants.MORTGAGE_TERM },
                    new Liability { Name = "Student Loans", Amount = 8000, InterestRate = 0.05M, Term = FinancialConstants.STUDENT_LOANS_TERM },
                    new Liability { Name = "Credit Card", Amount = 3000, InterestRate = 0.18M, Term = FinancialConstants.CREDIT_CARD_TERM },
                ]
            },
            new Profession
            {
                Name = "Software Architect", Salary = 4200, ChildExpense = 200, OtherExpenses = 900, Savings = 800,
                Liabilities = [
                    new Liability { Name = "Car", Amount = 7000, InterestRate = 0.05M, Term = FinancialConstants.CAR_LOAN_TERM },
                    new Liability { Name = "Mortgage", Amount = 65000, InterestRate = 0.06M, Term = FinancialConstants.MORTGAGE_TERM },
                    new Liability { Name = "Student Loans", Amount = 12000, InterestRate = 0.05M, Term = FinancialConstants.STUDENT_LOANS_TERM },
                    new Liability { Name = "Credit Card", Amount = 5000, InterestRate = 0.18M, Term = FinancialConstants.CREDIT_CARD_TERM },
                ]
            },
            new Profession
            {
                Name = "Software Tester", Salary = 2400, ChildExpense = 140, OtherExpenses = 580, Savings = 300,
                Liabilities = [
                    new Liability { Name = "Car", Amount = 3500, InterestRate = 0.05M, Term = FinancialConstants.CAR_LOAN_TERM },
                    new Liability { Name = "Mortgage", Amount = 32000, InterestRate = 0.06M, Term = FinancialConstants.MORTGAGE_TERM },
                    new Liability { Name = "Student Loans", Amount = 5000, InterestRate = 0.05M, Term = FinancialConstants.STUDENT_LOANS_TERM },
                    new Liability { Name = "Credit Card", Amount = 2000, InterestRate = 0.18M, Term = FinancialConstants.CREDIT_CARD_TERM },
                ]
            },
            new Profession
            {
                Name = "Software Analyst", Salary = 2600, ChildExpense = 150, OtherExpenses = 620, Savings = 350,
                Liabilities = [
                    new Liability { Name = "Car", Amount = 4500, InterestRate = 0.05M, Term = FinancialConstants.CAR_LOAN_TERM },
                    new Liability { Name = "Mortgage", Amount = 36000, InterestRate = 0.06M, Term = FinancialConstants.MORTGAGE_TERM },
                    new Liability { Name = "Student Loans", Amount = 7000, InterestRate = 0.05M, Term = FinancialConstants.STUDENT_LOANS_TERM },
                    new Liability { Name = "Credit Card", Amount = 2500, InterestRate = 0.18M, Term = FinancialConstants.CREDIT_CARD_TERM },
                ]
            },
            new Profession
            {
                Name = "Software Manager", Salary = 4800, ChildExpense = 220, OtherExpenses = 1050, Savings = 1000,
                Liabilities = [
                    new Liability { Name = "Car", Amount = 8000, InterestRate = 0.05M, Term = FinancialConstants.CAR_LOAN_TERM },
                    new Liability { Name = "Mortgage", Amount = 75000, InterestRate = 0.06M, Term = FinancialConstants.MORTGAGE_TERM },
                    new Liability { Name = "Student Loans", Amount = 10000, InterestRate = 0.05M, Term = FinancialConstants.STUDENT_LOANS_TERM },
                    new Liability { Name = "Credit Card", Amount = 6000, InterestRate = 0.18M, Term = FinancialConstants.CREDIT_CARD_TERM },
                ]
            },
            new Profession
            {
                Name = "Software Consultant", Salary = 3600, ChildExpense = 180, OtherExpenses = 800, Savings = 600,
                Liabilities = [
                    new Liability { Name = "Car", Amount = 6000, InterestRate = 0.05M, Term = FinancialConstants.CAR_LOAN_TERM },
                    new Liability { Name = "Mortgage", Amount = 52000, InterestRate = 0.06M, Term = FinancialConstants.MORTGAGE_TERM },
                    new Liability { Name = "Student Loans", Amount = 9000, InterestRate = 0.05M, Term = FinancialConstants.STUDENT_LOANS_TERM },
                    new Liability { Name = "Credit Card", Amount = 4000, InterestRate = 0.18M, Term = FinancialConstants.CREDIT_CARD_TERM },
                ]
            },
            new Profession
            {
                Name = "Janitor", Salary = 1600, ChildExpense = 100, OtherExpenses = 300, Savings = 560,
                Liabilities = [
                    new Liability { Name = "Car", Amount = 2000, InterestRate = 0.05M, Term = FinancialConstants.CAR_LOAN_TERM },
                    new Liability { Name = "Mortgage", Amount = 20000, InterestRate = 0.06M, Term = FinancialConstants.MORTGAGE_TERM },
                    new Liability { Name = "Credit Card", Amount = 1000, InterestRate = 0.18M, Term = FinancialConstants.CREDIT_CARD_TERM },
                ]
            },
            new Profession
            {
                Name = "Secretary", Salary = 2500, ChildExpense = 140, OtherExpenses = 570, Savings = 710,
                Liabilities = [
                    new Liability { Name = "Car", Amount = 3000, InterestRate = 0.05M, Term = FinancialConstants.CAR_LOAN_TERM },
                    new Liability { Name = "Mortgage", Amount = 38000, InterestRate = 0.06M, Term = FinancialConstants.MORTGAGE_TERM },
                    new Liability { Name = "Student Loans", Amount = 3000, InterestRate = 0.05M, Term = FinancialConstants.STUDENT_LOANS_TERM },
                    new Liability { Name = "Credit Card", Amount = 2000, InterestRate = 0.18M, Term = FinancialConstants.CREDIT_CARD_TERM },
                ]
            },
            new Profession
            {
                Name = "Truck Driver", Salary = 2500, ChildExpense = 140, OtherExpenses = 570, Savings = 750,
                Liabilities = [
                    new Liability { Name = "Car", Amount = 3500, InterestRate = 0.05M, Term = FinancialConstants.CAR_LOAN_TERM },
                    new Liability { Name = "Mortgage", Amount = 35000, InterestRate = 0.06M, Term = FinancialConstants.MORTGAGE_TERM },
                    new Liability { Name = "Credit Card", Amount = 2500, InterestRate = 0.18M, Term = FinancialConstants.CREDIT_CARD_TERM },
                ]
            },
            new Profession
            {
                Name = "Mechanic", Salary = 2000, ChildExpense = 110, OtherExpenses = 450, Savings = 670,
                Liabilities = [
                    new Liability { Name = "Car", Amount = 2500, InterestRate = 0.05M, Term = FinancialConstants.CAR_LOAN_TERM },
                    new Liability { Name = "Mortgage", Amount = 24000, InterestRate = 0.06M, Term = FinancialConstants.MORTGAGE_TERM },
                    new Liability { Name = "Credit Card", Amount = 1500, InterestRate = 0.18M, Term = FinancialConstants.CREDIT_CARD_TERM },
                ]
            },
            new Profession
            {
                Name = "Nurse", Salary = 3100, ChildExpense = 170, OtherExpenses = 710, Savings = 480,
                Liabilities = [
                    new Liability { Name = "Car", Amount = 5000, InterestRate = 0.05M, Term = FinancialConstants.CAR_LOAN_TERM },
                    new Liability { Name = "Mortgage", Amount = 47000, InterestRate = 0.06M, Term = FinancialConstants.MORTGAGE_TERM },
                    new Liability { Name = "Student Loans", Amount = 6000, InterestRate = 0.05M, Term = FinancialConstants.STUDENT_LOANS_TERM },
                    new Liability { Name = "Credit Card", Amount = 3000, InterestRate = 0.18M, Term = FinancialConstants.CREDIT_CARD_TERM },
                ]
            },
            new Profession
            {
                Name = "Teacher", Salary = 3300, ChildExpense = 180, OtherExpenses = 760, Savings = 400,
                Liabilities = [
                    new Liability { Name = "Car", Amount = 5000, InterestRate = 0.05M, Term = FinancialConstants.CAR_LOAN_TERM },
                    new Liability { Name = "Mortgage", Amount = 50000, InterestRate = 0.06M, Term = FinancialConstants.MORTGAGE_TERM },
                    new Liability { Name = "Student Loans", Amount = 12000, InterestRate = 0.05M, Term = FinancialConstants.STUDENT_LOANS_TERM },
                    new Liability { Name = "Credit Card", Amount = 3000, InterestRate = 0.18M, Term = FinancialConstants.CREDIT_CARD_TERM },
                ]
            },
            new Profession
            {
                Name = "Police Officer", Salary = 3000, ChildExpense = 160, OtherExpenses = 690, Savings = 520,
                Liabilities = [
                    new Liability { Name = "Car", Amount = 4000, InterestRate = 0.05M, Term = FinancialConstants.CAR_LOAN_TERM },
                    new Liability { Name = "Mortgage", Amount = 46000, InterestRate = 0.06M, Term = FinancialConstants.MORTGAGE_TERM },
                    new Liability { Name = "Credit Card", Amount = 3000, InterestRate = 0.18M, Term = FinancialConstants.CREDIT_CARD_TERM },
                ]
            },
            new Profession
            {
                Name = "Airline Pilot", Salary = 9500, ChildExpense = 480, OtherExpenses = 2210, Savings = 3000,
                Liabilities = [
                    new Liability { Name = "Car", Amount = 15000, InterestRate = 0.05M, Term = FinancialConstants.CAR_LOAN_TERM },
                    new Liability { Name = "Mortgage", Amount = 142000, InterestRate = 0.06M, Term = FinancialConstants.MORTGAGE_TERM },
                    new Liability { Name = "Student Loans", Amount = 18000, InterestRate = 0.05M, Term = FinancialConstants.STUDENT_LOANS_TERM },
                    new Liability { Name = "Credit Card", Amount = 8000, InterestRate = 0.18M, Term = FinancialConstants.CREDIT_CARD_TERM },
                ]
            },
            new Profession
            {
                Name = "Lawyer", Salary = 7500, ChildExpense = 380, OtherExpenses = 1650, Savings = 2100,
                Liabilities = [
                    new Liability { Name = "Car", Amount = 11000, InterestRate = 0.05M, Term = FinancialConstants.CAR_LOAN_TERM },
                    new Liability { Name = "Mortgage", Amount = 115000, InterestRate = 0.06M, Term = FinancialConstants.MORTGAGE_TERM },
                    new Liability { Name = "Student Loans", Amount = 78000, InterestRate = 0.05M, Term = FinancialConstants.STUDENT_LOANS_TERM },
                    new Liability { Name = "Credit Card", Amount = 6000, InterestRate = 0.18M, Term = FinancialConstants.CREDIT_CARD_TERM },
                ]
            },
            new Profession
            {
                Name = "Doctor", Salary = 13200, ChildExpense = 640, OtherExpenses = 2880, Savings = 3950,
                Liabilities = [
                    new Liability { Name = "Car", Amount = 19000, InterestRate = 0.05M, Term = FinancialConstants.CAR_LOAN_TERM },
                    new Liability { Name = "Mortgage", Amount = 202000, InterestRate = 0.06M, Term = FinancialConstants.MORTGAGE_TERM },
                    new Liability { Name = "Student Loans", Amount = 150000, InterestRate = 0.05M, Term = FinancialConstants.STUDENT_LOANS_TERM },
                    new Liability { Name = "Credit Card", Amount = 9000, InterestRate = 0.18M, Term = FinancialConstants.CREDIT_CARD_TERM },
                ]
            },
            new Profession
            {
                Name = "Business Manager", Salary = 4600, ChildExpense = 240, OtherExpenses = 1020, Savings = 900,
                Liabilities = [
                    new Liability { Name = "Car", Amount = 7000, InterestRate = 0.05M, Term = FinancialConstants.CAR_LOAN_TERM },
                    new Liability { Name = "Mortgage", Amount = 75000, InterestRate = 0.06M, Term = FinancialConstants.MORTGAGE_TERM },
                    new Liability { Name = "Student Loans", Amount = 12000, InterestRate = 0.05M, Term = FinancialConstants.STUDENT_LOANS_TERM },
                    new Liability { Name = "Credit Card", Amount = 5000, InterestRate = 0.18M, Term = FinancialConstants.CREDIT_CARD_TERM },
                ]
            },
            new Profession
            {
                Name = "Engineer", Salary = 4900, ChildExpense = 250, OtherExpenses = 1090, Savings = 1200,
                Liabilities = [
                    new Liability { Name = "Car", Amount = 7500, InterestRate = 0.05M, Term = FinancialConstants.CAR_LOAN_TERM },
                    new Liability { Name = "Mortgage", Amount = 75000, InterestRate = 0.06M, Term = FinancialConstants.MORTGAGE_TERM },
                    new Liability { Name = "Student Loans", Amount = 12000, InterestRate = 0.05M, Term = FinancialConstants.STUDENT_LOANS_TERM },
                    new Liability { Name = "Credit Card", Amount = 5000, InterestRate = 0.18M, Term = FinancialConstants.CREDIT_CARD_TERM },
                ]
            }
        ];
    }
}