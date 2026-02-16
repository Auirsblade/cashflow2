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
        AssignRandomEmoji(player, []);
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
            existingPlayer.IsActive = true;
            gameService.UpdateGame(game);
            await Groups.AddToGroupAsync(Context.ConnectionId, game.Code);
            await Clients.OthersInGroup(game.Code).GameStateUpdated(game);
            return new GameResponse { Player = existingPlayer, Game = game, PlayerOptions = new PlayerOptions() };
        }

        Player player = new(playerName);
        AssignRandomEmoji(player, game.Players);
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

    public async Task PayDoodad(string gameCode, Guid playerId, bool useCard)
    {
        if (await ValidateGameExistence(gameCode) is not { } game) return;
        if (await ValidatePlayerExistence(game, playerId) is not { } player) return;
        if (!ValidateCurrentTurn(game, player)) return;

        gameService.PayDoodad(game, player, useCard);

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

    public async Task BuyDealWithLoan(string gameCode, Guid playerId, int loanTerm)
    {
        if (await ValidateGameExistence(gameCode) is not { } game) return;
        if (await ValidatePlayerExistence(game, playerId) is not { } player) return;
        if (!ValidateCurrentTurn(game, player)) return;

        gameService.BuyDealWithLoan(game, player, loanTerm);

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

    public async Task PlaceBid(string gameCode, Guid playerId, decimal bidAmount)
    {
        if (await ValidateGameExistence(gameCode) is not { } game) return;
        if (await ValidatePlayerExistence(game, playerId) is not { } player) return;

        gameService.PlaceBid(game, player, bidAmount);

        await Clients.Group(game.Code).GameStateUpdated(game);
    }

    public async Task AuctionPass(string gameCode, Guid playerId)
    {
        if (await ValidateGameExistence(gameCode) is not { } game) return;
        if (await ValidatePlayerExistence(game, playerId) is not { } player) return;

        gameService.AuctionPass(game, player);

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

    public async Task BuyStock(string gameCode, Guid playerId, string ticker, int quantity)
    {
        if (await ValidateGameExistence(gameCode) is not { } game) return;
        if (await ValidatePlayerExistence(game, playerId) is not { } player) return;

        gameService.BuyStock(game, player, ticker, quantity);

        await Clients.Group(game.Code).GameStateUpdated(game);
    }

    public async Task SellStock(string gameCode, Guid playerId, string ticker, int quantity)
    {
        if (await ValidateGameExistence(gameCode) is not { } game) return;
        if (await ValidatePlayerExistence(game, playerId) is not { } player) return;

        gameService.SellStock(game, player, ticker, quantity);

        await Clients.Group(game.Code).GameStateUpdated(game);
    }

    public async Task TakeOutLoan(string gameCode, Guid playerId, decimal amount, int term)
    {
        if (await ValidateGameExistence(gameCode) is not { } game) return;
        if (await ValidatePlayerExistence(game, playerId) is not { } player) return;

        gameService.TakeOutLoan(game, player, amount, term);

        await Clients.Group(game.Code).GameStateUpdated(game);
    }

    public async Task PayOffLoan(string gameCode, Guid playerId, Guid liabilityId, decimal amount)
    {
        if (await ValidateGameExistence(gameCode) is not { } game) return;
        if (await ValidatePlayerExistence(game, playerId) is not { } player) return;

        gameService.PayOffLoan(game, player, liabilityId, amount);

        await Clients.Group(game.Code).GameStateUpdated(game);
    }

    public async Task SetEmoji(string gameCode, Guid playerId, string emoji)
    {
        if (await ValidateGameExistence(gameCode) is not { } game) return;
        if (await ValidatePlayerExistence(game, playerId) is not { } player) return;

        if (game.Players.Any(p => p.Id != playerId && p.Emoji == emoji))
        {
            await Clients.Client(Context.ConnectionId).Error("Emoji already taken");
            return;
        }

        player.Emoji = emoji;
        gameService.UpdateGame(game);

        await Clients.Group(game.Code).GameStateUpdated(game);
    }

    public async Task RemovePlayer(string gameCode, Guid adminId, Guid targetPlayerId)
    {
        if (await ValidateGameExistence(gameCode) is not { } game) return;
        if (game.CreatorId != adminId)
        {
            await Clients.Client(Context.ConnectionId).Error("Only the game creator can remove players");
            return;
        }
        if (await ValidatePlayerExistence(game, targetPlayerId) is not { } targetPlayer) return;
        if (targetPlayer.Id == adminId)
        {
            await Clients.Client(Context.ConnectionId).Error("Cannot remove yourself");
            return;
        }

        gameService.RemovePlayer(game, targetPlayer);

        await Clients.Group(game.Code).GameStateUpdated(game);
    }

    public async Task LeaveGame(string gameCode, Guid playerId)
    {
        if (await ValidateGameExistence(gameCode) is not { } game) return;
        if (await ValidatePlayerExistence(game, playerId) is not { } player) return;

        gameService.RemovePlayer(game, player);

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

    private static void AssignRandomEmoji(Player player, List<Player> existingPlayers)
    {
        var taken = existingPlayers.Select(p => p.Emoji).ToHashSet();
        string[] available = EmojiList.Available.Where(e => !taken.Contains(e)).ToArray();
        player.Emoji = available.Length > 0
            ? available[Random.Shared.Next(available.Length)]
            : EmojiList.Available[Random.Shared.Next(EmojiList.Available.Length)];
    }
}