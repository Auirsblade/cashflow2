using System.Text.Json;
using System.Text.Json.Serialization;

namespace Cashflow.API.Entities;

public class Game
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Code { get; set; } = Utility.RandomAlphanumericString(4);
    public List<Player> Players { get; set; } = [];
    public List<BoardSpace>? BoardSpaces { get; set; }

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