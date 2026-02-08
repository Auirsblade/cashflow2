using Cashflow.API.Entities;

namespace Cashflow.API.Services;

public static class StockMarketService
{
    private static readonly List<StockDefinition> Definitions =
    [
        new() { Ticker = "MEME", Name = "MemeKing Inc.", Category = StockCategory.Penny, StartingPrice = 8m, StdDev = 0.15, DividendYield = 0m },
        new() { Ticker = "YOLO", Name = "YOLO Therapeutics", Category = StockCategory.Penny, StartingPrice = 14m, StdDev = 0.18, DividendYield = 0.02m },
        new() { Ticker = "BURG", Name = "BurgerVerse Corp.", Category = StockCategory.BlueChip, StartingPrice = 45m, StdDev = 0.04, DividendYield = 0.03m },
        new() { Ticker = "SOCK", Name = "SockDraw Holdings", Category = StockCategory.BlueChip, StartingPrice = 62m, StdDev = 0.03, DividendYield = 0.025m },
        new() { Ticker = "BEAN", Name = "MagicBean Financial", Category = StockCategory.BlueChip, StartingPrice = 38m, StdDev = 0.05, DividendYield = 0m },
        new() { Ticker = "NAPS", Name = "NapTime Industries", Category = StockCategory.BlueChip, StartingPrice = 75m, StdDev = 0.035, DividendYield = 0.02m },
        new() { Ticker = "GLUE", Name = "SuperGlue Systems", Category = StockCategory.BlueChip, StartingPrice = 55m, StdDev = 0.04, DividendYield = 0m },
        new() { Ticker = "DUCK", Name = "RubberDuck Energy", Category = StockCategory.BlueChip, StartingPrice = 50m, StdDev = 0.045, DividendYield = 0m },
        new() { Ticker = "TOTL", Name = "Total Chaos ETF", Category = StockCategory.ETF, StartingPrice = 0m, StdDev = 0, DividendYield = 0m, ComponentTickers = ["MEME", "YOLO", "BURG", "SOCK", "BEAN", "NAPS", "GLUE", "DUCK"] },
        new() { Ticker = "BLUE", Name = "Blue Chip Bliss ETF", Category = StockCategory.ETF, StartingPrice = 0m, StdDev = 0, DividendYield = 0m, ComponentTickers = ["BURG", "SOCK", "BEAN", "NAPS", "GLUE", "DUCK"] },
    ];

    public static StockMarket InitializeMarket()
    {
        var market = new StockMarket();

        foreach (var def in Definitions)
        {
            market.Stocks.Add(new StockState
            {
                Ticker = def.Ticker,
                Name = def.Name,
                Category = def.Category,
                CurrentPrice = def.StartingPrice,
                PreviousPrice = def.StartingPrice,
                DividendYield = def.DividendYield,
                ComponentTickers = def.ComponentTickers
            });
        }

        RecalculateETFPrices(market);

        return market;
    }

    public static void UpdatePrices(StockMarket market)
    {
        var rng = new Random();

        foreach (var stock in market.Stocks)
        {
            if (stock.Category == StockCategory.ETF) continue;

            var def = Definitions.First(d => d.Ticker == stock.Ticker);
            double change = NextGaussian(rng, 0.005, def.StdDev);
            change = Math.Clamp(change, -0.90, 9.0);

            stock.PreviousPrice = stock.CurrentPrice;
            stock.CurrentPrice = Math.Max(0.01m, Math.Round(stock.CurrentPrice * (1 + (decimal)change), 2));
        }

        RecalculateETFPrices(market);
    }

    public static void RecalculateETFPrices(StockMarket market)
    {
        foreach (var etf in market.Stocks.Where(s => s.Category == StockCategory.ETF))
        {
            if (etf.ComponentTickers == null || etf.ComponentTickers.Count == 0) continue;

            var components = market.Stocks.Where(s => etf.ComponentTickers.Contains(s.Ticker)).ToList();
            decimal oldPrice = etf.CurrentPrice;
            etf.PreviousPrice = oldPrice;
            etf.CurrentPrice = Math.Round(components.Average(c => c.CurrentPrice), 2);
            etf.DividendYield = components.Sum(c => c.DividendYield) / components.Count;
        }
    }

    public static bool BuyStock(Game game, Player player, string ticker, int quantity)
    {
        if (quantity <= 0) return false;

        var stock = game.StockMarket.Stocks.FirstOrDefault(s => s.Ticker == ticker);
        if (stock == null) return false;

        decimal totalCost = stock.CurrentPrice * quantity;
        if (totalCost > player.Cash) return false;

        player.Cash -= totalCost;

        var position = player.StockPositions.FirstOrDefault(p => p.Ticker == ticker);
        if (position != null)
        {
            decimal totalExistingCost = position.AverageCost * position.Quantity;
            position.Quantity += quantity;
            position.AverageCost = Math.Round((totalExistingCost + totalCost) / position.Quantity, 2);
        }
        else
        {
            player.StockPositions.Add(new StockPosition
            {
                Ticker = ticker,
                Quantity = quantity,
                AverageCost = stock.CurrentPrice
            });
        }

        RecalculatePlayerDividends(player, game.StockMarket);
        return true;
    }

    public static bool SellStock(Game game, Player player, string ticker, int quantity)
    {
        if (quantity <= 0) return false;

        var position = player.StockPositions.FirstOrDefault(p => p.Ticker == ticker);
        if (position == null || position.Quantity < quantity) return false;

        var stock = game.StockMarket.Stocks.FirstOrDefault(s => s.Ticker == ticker);
        if (stock == null) return false;

        decimal proceeds = stock.CurrentPrice * quantity;
        player.Cash += proceeds;

        position.Quantity -= quantity;
        if (position.Quantity == 0)
        {
            player.StockPositions.Remove(position);
        }

        RecalculatePlayerDividends(player, game.StockMarket);
        return true;
    }

    public static void RecalculatePlayerDividends(Player player, StockMarket market)
    {
        decimal totalDividendIncome = 0;

        foreach (var position in player.StockPositions)
        {
            var stock = market.Stocks.FirstOrDefault(s => s.Ticker == position.Ticker);
            if (stock == null || stock.DividendYield == 0) continue;

            totalDividendIncome += position.Quantity * stock.CurrentPrice * stock.DividendYield / 12;
        }

        player.DividendIncome = Math.Round(totalDividendIncome, 2);
    }

    public static void RecalculateAllDividends(Game game)
    {
        foreach (var player in game.Players)
        {
            RecalculatePlayerDividends(player, game.StockMarket);
        }
    }

    private static double NextGaussian(Random rng, double mean, double stdDev)
    {
        double u1 = 1.0 - rng.NextDouble();
        double u2 = 1.0 - rng.NextDouble();
        double normal = Math.Sqrt(-2.0 * Math.Log(u1)) * Math.Sin(2.0 * Math.PI * u2);
        return mean + stdDev * normal;
    }
}
