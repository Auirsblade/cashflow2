using Cashflow.API.Entities;

namespace Cashflow.API.Services;

public static class LoanService
{
    public static bool TakeOutLoan(Player player, decimal amount, int term)
    {
        if (amount <= 0) return false;
        if (term < 1 || term > 5) return false;

        var liability = new Liability
        {
            Name = "Personal Loan",
            Amount = amount,
            InterestRate = 0.10m,
            Term = term * FinancialConstants.PAYMENTS_PER_ROUND
        };

        player.Liabilities.Add(liability);
        player.Cash += amount;

        return true;
    }

    public static bool PayOffLoan(Player player, Guid liabilityId, decimal amount)
    {
        if (amount <= 0) return false;

        var liability = player.Liabilities.FirstOrDefault(l => l.Id == liabilityId);
        if (liability == null) return false;

        amount = Math.Min(amount, liability.Amount);
        if (amount > player.Cash) return false;

        player.Cash -= amount;
        liability.Amount -= amount;

        if (liability.Amount <= 0)
        {
            player.Liabilities.Remove(liability);
        }

        return true;
    }
}
