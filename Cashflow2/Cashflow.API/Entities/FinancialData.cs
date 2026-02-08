namespace Cashflow.API.Entities;

public static class FinancialConstants
{
    public const int PAYMENTS_PER_ROUND = 12;
    public const int MORTGAGE_TERM = 30 * PAYMENTS_PER_ROUND;
    public const int CAR_LOAN_TERM = 5 * PAYMENTS_PER_ROUND;
    public const int STUDENT_LOANS_TERM = 20 * PAYMENTS_PER_ROUND;
    public const int CREDIT_CARD_TERM = 0;
}

public enum AssetType
{
    mlm1,
    mlm2,
    business,
    twoOne,
    threeTwo,
    apartment,
    cd,
    land,
    gold
}

public class Asset
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Name { get; set; }
    public AssetType Type { get; set; }
    public int? Quantity { get; set; }

    /// <summary>
    /// Equity in the Asset, also used as Cost
    /// </summary>
    public decimal Equity { get; set; }
    public decimal Value { get; set; }
    public decimal RateOfReturn { get; set; }
    public decimal LoanAmount => Value - Equity;
    public decimal Income => Value * (RateOfReturn / FinancialConstants.PAYMENTS_PER_ROUND);
}

public class Liability
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Name { get; set; }
    public decimal Amount { get; set; }
    public decimal InterestRate { get; set; }
    public int Term { get; set; }

    public decimal Expense
    {
        get
        {
            if (Amount <= 0) return 0;
            if (Term <= 0 && InterestRate > 0)
                return Math.Round(Amount * InterestRate / FinancialConstants.PAYMENTS_PER_ROUND, 2);
            if (Term <= 0) return 0;
            if (InterestRate == 0) return Math.Round(Amount / Term, 2);
            decimal r = InterestRate / FinancialConstants.PAYMENTS_PER_ROUND;
            return Math.Round(Amount * r / (1.0M - (decimal)Math.Pow(1.0 + (double)r, -Term)), 2);
        }
    }

    public bool ApplyMonthlyAmortization()
    {
        if (Term <= 0) return false;

        decimal monthlyPayment = Expense;

        if (InterestRate == 0)
        {
            Amount -= monthlyPayment;
        }
        else
        {
            decimal r = InterestRate / FinancialConstants.PAYMENTS_PER_ROUND;
            Amount = Amount * (1 + r) - monthlyPayment;
        }

        Term--;

        if (Amount <= 0.01M || Term <= 0)
        {
            Amount = 0;
            return true;
        }

        return false;
    }
}

public class PurchaseOffer
{
    public string Name { get; set; }
    public AssetType Type { get; set; }
    public decimal Price { get; set; }
}