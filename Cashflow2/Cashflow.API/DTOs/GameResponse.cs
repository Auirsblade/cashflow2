using Cashflow.API.Entities;

namespace Cashflow.API.DTOs;

public class GameResponse
{
    public Player CurrentPlayer { get; set; }
    public Game game { get; set; }
}