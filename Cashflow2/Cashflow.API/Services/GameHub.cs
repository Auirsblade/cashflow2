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

        // Check if player with the same name already exists in the game
        Player? existingPlayer = game.Players.Find(x => x.Name.Equals(playerName, StringComparison.OrdinalIgnoreCase));
        
        if (existingPlayer != null)
        {
            // Player already exists, return existing player instead of creating new one
            await Groups.AddToGroupAsync(Context.ConnectionId, game.Code);
            return new GameResponse { Player = existingPlayer, Game = game, PlayerOptions = new PlayerOptions() };
        }

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
        if (!ValidateCurrentTurn(game, player)) return;

        gameService.MovePlayer(game, player, spacesToMove);

        await Clients.Group(game.Code).GameStateUpdated(game);
    }

    public async Task EndTurn(string gameCode, Guid playerId)
    {
        if (await ValidateGameExistence(gameCode) is not { } game) return;
        if (await ValidatePlayerExistence(game, playerId) is not { } player) return;
        if (!ValidateCurrentTurn(game, player)) return;

        gameService.EndTurn(game, player);

        await Clients.Group(game.Code).GameStateUpdated(game);
    }

    public async Task BuyCharity(string gameCode, Guid playerId)
    {
        if (await ValidateGameExistence(gameCode) is not { } game) return;
        if (await ValidatePlayerExistence(game, playerId) is not { } player) return;
        if (!ValidateCurrentTurn(game, player)) return;

        gameService.BuyCharity(game, player);

        await Clients.Group(game.Code).GameStateUpdated(game);
    }

    public async Task GetDeal(string gameCode, Guid playerId, bool isBig)
    {
        if (await ValidateGameExistence(gameCode) is not { } game) return;
        if (await ValidatePlayerExistence(game, playerId) is not { } player) return;
        if (!ValidateCurrentTurn(game, player)) return;

        gameService.GetDeal(game, player, isBig);

        await Clients.Group(game.Code).GameStateUpdated(game);
    }

    public async Task BuyDeal(string gameCode, Guid playerId)
    {
        if (await ValidateGameExistence(gameCode) is not { } game) return;
        if (await ValidatePlayerExistence(game, playerId) is not { } player) return;
        if (!ValidateCurrentTurn(game, player)) return;

        gameService.BuyDeal(game, player);

        await Clients.Group(game.Code).GameStateUpdated(game);
    }

    public async Task SellDeal(string gameCode, Guid playerId)
    {
        if (await ValidateGameExistence(gameCode) is not { } game) return;
        if (await ValidatePlayerExistence(game, playerId) is not { } player) return;
        if (!ValidateCurrentTurn(game, player)) return;

        gameService.SellDeal(game, player);

        await Clients.Group(game.Code).GameStateUpdated(game);
    }

    public async Task SellToMarket(string gameCode, Guid playerId, Guid assetId)
    {
        if (await ValidateGameExistence(gameCode) is not { } game) return;
        if (await ValidatePlayerExistence(game, playerId) is not { } player) return;
        if (await ValidateAssetExistence(player, assetId) is not { } asset) return;

        gameService.SellToMarket(game, player, asset);

        await Clients.Group(game.Code).GameStateUpdated(game);
    }

    public async Task MarketPass(string gameCode, Guid playerId)
    {
        if (await ValidateGameExistence(gameCode) is not { } game) return;
        if (await ValidatePlayerExistence(game, playerId) is not { } player) return;

        gameService.MarketPass(game, player);

        await Clients.Group(game.Code).GameStateUpdated(game);
    }

    private static bool ValidateCurrentTurn(Game game, Player player)
    {
        return player.Id == game.CurrentPlayerId;
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

    private async Task<Asset?> ValidateAssetExistence(Player player, Guid assetId)
    {
        Asset? asset = player.Assets.Find(x => x.Id == assetId);
        if (asset != null) return asset;

        await Clients.Client(Context.ConnectionId).Error("Asset not found");
        return null;
    }
}