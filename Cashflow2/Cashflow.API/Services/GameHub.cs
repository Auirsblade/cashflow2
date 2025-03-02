using Cashflow.API.DTOs;
using Cashflow.API.Entities;
using Microsoft.AspNetCore.SignalR;
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

        return new GameResponse { Player = player, Game = game, PlayerOptions = new PlayerOptions() };
    }

    public async Task<GameResponse?> JoinGame(string playerName, string gameCode)
    {
        if (await ValidateGameExistence(gameCode) is not { } game) return null;

        Player player = new(playerName);
        gameService.JoinGame(player, game);

        await Groups.AddToGroupAsync(Context.ConnectionId, game.Code);
        await Clients.OthersInGroup(game.Code).GameStateUpdated(game);

        return new GameResponse { Player = player, Game = game, PlayerOptions = new PlayerOptions()};
    }

    public async Task SelectProfession(string gameCode, Guid playerId, Profession profession)
    {
        if (await ValidateGameExistence(gameCode) is not { } game) return;
        if (await ValidatePlayerExistence(game, playerId) is not { } player) return;

        gameService.SelectProfession(game, player, profession);

        await Clients.Group(game.Code).GameStateUpdated(game);
    }

    public async Task MovePlayer(string gameCode, Guid playerId, int spacesToMove)
    {
        if (await ValidateGameExistence(gameCode) is not { } game) return;
        if (await ValidatePlayerExistence(game, playerId) is not { } player) return;

        gameService.MovePlayer(game, player, spacesToMove);

        await Clients.Group(game.Code).GameStateUpdated(game);
    }

    public async Task EndTurn(string gameCode, Guid playerId)
    {
        if (await ValidateGameExistence(gameCode) is not { } game) return;
        if (await ValidatePlayerExistence(game, playerId) is not { } player) return;

        gameService.EndTurn(game, player);

        await Clients.Group(game.Code).GameStateUpdated(game);
    }

    public async Task BuyCharity(string gameCode, Guid playerId)
    {
        if (await ValidateGameExistence(gameCode) is not { } game) return;
        if (await ValidatePlayerExistence(game, playerId) is not { } player) return;

        gameService.BuyCharity(game, player);

        await Clients.Group(game.Code).GameStateUpdated(game);
    }

    public async Task GetDeal(string gameCode, Guid playerId, bool isBig)
    {
        if (await ValidateGameExistence(gameCode) is not { } game) return;
        if (await ValidatePlayerExistence(game, playerId) is not { } player) return;

        gameService.GetDeal(game, player, isBig);

        await Clients.Group(game.Code).GameStateUpdated(game);
    }

    private async Task<Game?> ValidateGameExistence(string gameCode)
    {
        Game? game = gameService.GetGame(gameCode);
        if (game != null) return game;

        await Clients.Client(Context.ConnectionId).Error("Game not found");
        return null;
    }

    private async Task<Player?> ValidatePlayerExistence(Game game, Guid playerId)
    {
        Player? player = game.Players.Find(x => x.Id == playerId);
        if (player != null) return player;

        await Clients.Client(Context.ConnectionId).Error("Player not found");
        return null;
    }
}