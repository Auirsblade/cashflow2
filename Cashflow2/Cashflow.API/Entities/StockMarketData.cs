namespace Cashflow.API.Entities;

public enum StockCategory { Penny, BlueChip, ETF }

public class StockDefinition
{
    public string Ticker { get; set; } = "";
    public string Name { get; set; } = "";
    public StockCategory Category { get; set; }
    public decimal StartingPrice { get; set; }
    public double StdDev { get; set; }
    public decimal DividendYield { get; set; }
    public List<string>? ComponentTickers { get; set; }
}

public class StockState
{
    public string Ticker { get; set; } = "";
    public string Name { get; set; } = "";
    public StockCategory Category { get; set; }
    public decimal CurrentPrice { get; set; }
    public decimal PreviousPrice { get; set; }
    public decimal DividendYield { get; set; }
    public List<string>? ComponentTickers { get; set; }

    public decimal Change => CurrentPrice - PreviousPrice;
    public decimal ChangePercent => PreviousPrice != 0 ? Math.Round((CurrentPrice - PreviousPrice) / PreviousPrice * 100, 2) : 0;
}

public class StockPosition
{
    public string Ticker { get; set; } = "";
    public int Quantity { get; set; }
    public decimal AverageCost { get; set; }
}

public class StockMarket
{
    public List<StockState> Stocks { get; set; } = new();
    public int TurnNumber { get; set; }
}
