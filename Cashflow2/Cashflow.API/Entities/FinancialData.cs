namespace Cashflow.API.Entities;

public static class FinancialConstants
{
    public const int PAYMENTS_PER_ROUND = 12;
    public const int MORTGAGE_TERM = 30;
    public const int CAR_LOAN_TERM = 5;
    public const int STUDENT_LOANS_TERM = 20;
    public const int CREDIT_CARD_TERM = 2;
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
    public string Name { get; set; }
    public decimal Amount { get; set; }
    public decimal InterestRate { get; set; }
    public int Term { get; set; }

    public decimal Expense =>
        Math.Round(Amount
                   * (InterestRate / FinancialConstants.PAYMENTS_PER_ROUND)
                   / (1.0M - (decimal)Math.Pow(1.0 / 1.0 + (double)InterestRate / FinancialConstants.PAYMENTS_PER_ROUND, -_payments)),
                   2);
    private int _payments => Term * FinancialConstants.PAYMENTS_PER_ROUND;
}

public class PurchaseOffer
{
    public string Name { get; set; }
    public AssetType Type { get; set; }
    public decimal Price { get; set; }
}