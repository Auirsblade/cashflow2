using Cashflow.API.DTOs;
using Cashflow.API.Entities;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Caching.Memory;
using SignalRSwaggerGen.Attributes;

namespace Cashflow.API.Services;

[SignalRHub]
public class GameHub(GameService gameService) : Hub<IGameClient>
{
    public async Task<GameResponse> CreateGame(string playerName)
    {
        Player player = new(playerName);
        Game game = gameService.CreateGame(player);

        await Groups.AddToGroupAsync(Context.ConnectionId, game.Code);

        return new GameResponse { Player = player, Game = game };
    }

    public async Task<GameResponse> JoinGame(string playerName, string gameCode)
    {
        Player player = new(playerName);
        Game? game = gameService.JoinGame(player, gameCode);
        if (game == null)
        {
            await Clients.Client(Context.ConnectionId).Error("Game not found");
            return new GameResponse { IsSuccess = false, Message = "Game not found" };
        }

        await Groups.AddToGroupAsync(Context.ConnectionId, game.Code);
        await Clients.OthersInGroup(game.Code).GameStateUpdated(game);

        return new GameResponse { Player = player, Game = game };
    }
}