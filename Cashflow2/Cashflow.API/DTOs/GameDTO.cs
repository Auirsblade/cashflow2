using Cashflow.API.Entities;

namespace Cashflow.API.DTOs;

public class GameResponse
{
    public bool IsSuccess { get; set; } = true;
    public string Message { get; set; }
    public Player Player { get; set; }
    public Game Game { get; set; }
}