﻿namespace Cashflow.API.Entities;

public class Player(string name)
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Name { get; set; } = name;
    public int BoardSpaceId { get; set; } = 4;
    public Profession Profession { get; set; }
    public List<Asset> Assets { get; set; } = new();
    public List<Liability> Liabilities { get; set; } = new();
    public int NumberOfChildren { get; set; } = 0;
    public decimal Cash { get; set; } = 0;
    public int CharityTurnsRemaining { get; set; } = 0;
    public int DownsizedTurnsRemaining { get; set; } = 0;

    // Calculated fields
    public decimal Income => Assets.Sum(x => x.Income) + (Profession?.Salary ?? 0);
    public decimal Taxes => Income * 0.20M;
    public decimal ChildExpenses => (Profession?.ChildExpense ?? 0) * NumberOfChildren;
    public decimal Expenses => Liabilities.Sum(x => x.Expense) + ChildExpenses + (Profession?.OtherExpenses ?? 0);
    public decimal NetIncome => Income - Expenses;

    public void SetProfession(Profession profession)
    {
        Profession = profession;
        Cash = profession.Savings;
        Assets = profession.Assets;
        Liabilities = profession.Liabilities;
    }

    public void Payday()
    {
        Cash += NetIncome;
    }

    public void Downsized()
    {
        Cash -= Expenses;
        DownsizedTurnsRemaining = 2;
        CharityTurnsRemaining = 0;
    }

    public void HaveBaby()
    {
        // Why?
        if (NumberOfChildren < 3)
        {
            NumberOfChildren++;
        }
    }

    public void BuyDoodad(Doodad doodad)
    {
        Cash -= doodad.Cost;
    }

    public void BuyCharity()
    {
        CharityTurnsRemaining = 2;
        Cash -= Income * 0.10M;
    }
}

public class Profession
{
    public string Name { get; set; }
    public decimal Salary { get; set; }
    public decimal ChildExpense { get; set; }
    public decimal OtherExpenses { get; set; }
    public decimal Savings { get; set; }
    public List<Asset> Assets { get; set; } = new();
    public List<Liability> Liabilities { get; set; } = new();
}