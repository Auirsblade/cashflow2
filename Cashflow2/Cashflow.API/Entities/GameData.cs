using System.Text.Json;

namespace Cashflow.API.Entities;

public class Game
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Code { get; set; } = Utility.RandomAlphanumericString(4);
    public Guid CurrentPlayerId { get; set; }
    public List<Player> Players { get; set; } = [];
    public List<BoardSpace>? BoardSpaces { get; set; }
    public ConfirmAction? ConfirmAction { get; set; }
    public DealAction? DealAction { get; set; }
    public MarketAction? MarketAction { get; set; }
    public CharityAction? CharityAction { get; set; }

    public Game()
    {
        BoardSpaces = JsonSerializer.Deserialize<List<Board>>(File.ReadAllText(@"./Resources/Boards.json"))?.FirstOrDefault(x => x.Name == "Default")?.Spaces;
    }
}

public class Board
{
    public string? Name { get; set; }
    public List<BoardSpace>? Spaces { get; set; }
}

public class BoardSpace
{
    public int Id { get; set; }
    public string? Name { get; set; }
}

public enum ActionType
{
    Payday,
    Deal,
    Market,
    Doodad,
    Charity,
    Baby,
    Downsized
}

public abstract class GameAction
{
    public abstract ActionType Name { get; }
    public abstract bool IsAnyPlayer { get; }
    public string Title => Name.ToString();
}

public class ConfirmAction(ActionType name) : GameAction
{
    public override ActionType Name => name;
    public override bool IsAnyPlayer => false;
    public Doodad? Doodad { get; set; }
}

public class Doodad
{
    public string Name { get; set; }
    public decimal Cost { get; set; }
}

public class DealAction : GameAction
{
    public override ActionType Name => ActionType.Deal;
    public override bool IsAnyPlayer => false;
}

public class MarketAction : GameAction
{
    public override ActionType Name => ActionType.Market;
    public override bool IsAnyPlayer => true;
}

public class CharityAction : GameAction
{
    public override ActionType Name => ActionType.Charity;
    public override bool IsAnyPlayer => false;
}