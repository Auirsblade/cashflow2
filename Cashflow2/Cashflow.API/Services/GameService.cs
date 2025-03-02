using System.Text.Json;
using Cashflow.API.Entities;
using Microsoft.Extensions.Caching.Memory;

namespace Cashflow.API.Services;

public class GameService(IMemoryCache gameCache)
{
    public Game? GetGame(string gameCode) => gameCache.Get<Game>(gameCode.ToUpper());

    public Game CreateGame(Player creator)
    {
        Game game = new();
        game.Players.Add(creator);

        gameCache.Set(game.Code, game);

        return game;
    }

    public void JoinGame(Player player, Game game)
    {
        game.Players.Add(player);
        gameCache.Set(game.Code, game);
    }

    public void SelectProfession(Game game, Player player, Profession profession)
    {
        player.SetProfession(profession);
        gameCache.Set(game.Code, game);
    }

    public void MovePlayer(Game game, Player player, int spacesToMove)
    {
        int currentSpaceId = player.BoardSpaceId;
        int newSpaceId = getNewSpaceId(currentSpaceId, spacesToMove);
        if (game.BoardSpaces?.Any(x => x.Id > currentSpaceId && x.Id < newSpaceId && x.Name != null && x.Name.Equals("payday", StringComparison.OrdinalIgnoreCase)) ?? false)
        {
            player.Payday();
        }
        player.BoardSpaceId = newSpaceId;
        BoardSpace? boardSpace = game.BoardSpaces?.FirstOrDefault(x => x.Id == newSpaceId);

        switch (boardSpace?.Name?.ToLowerInvariant())
        {
            case "payday":
                player.Payday();
                game.ConfirmAction = new ConfirmAction(ActionType.Payday);
                break;
            case "deal":
                game.DealAction = new DealAction();
                break;
            case "market":
                game.MarketAction = new MarketAction();
                break;
            case "doodad":
                List<Doodad> doodads = JsonSerializer.Deserialize<List<Doodad>>(File.ReadAllText(@"./Resources/Doodads.json")) ?? [];
                Doodad doodad = doodads[new Random().Next(0, doodads.Count)];
                player.BuyDoodad(doodad);
                game.ConfirmAction = new ConfirmAction(ActionType.Doodad);
                break;
            case "charity":
                game.CharityAction = new CharityAction();
                break;
            case "baby":
                player.HaveBaby();
                game.ConfirmAction = new ConfirmAction(ActionType.Baby);
                break;
            case "downsized":
                player.Downsized();
                game.ConfirmAction = new ConfirmAction(ActionType.Downsized);
                break;
        }

        gameCache.Set(game.Code, game);
    }

    public void BuyCharity(Game game, Player player)
    {
        player.BuyCharity();
        CycleTurn(game, player);
        gameCache.Set(game.Code, game);
    }

    public void GetDeal(Game game, Player player, bool isBig)
    {
        Deals? deals = JsonSerializer.Deserialize<Deals>(File.ReadAllText(@"./Resources/Deals.json"));
        Random random = new();
        Asset? deal = isBig ? deals?.Big[random.Next(0, deals.Big.Count)] : deals?.Small[random.Next(0, deals.Small.Count)];
        if (game.DealAction != null) game.DealAction.Asset = deal;

        gameCache.Set(game.Code, game);
    }

    public void EndTurn(Game game, Player player)
    {
        if (player.CharityTurnsRemaining > 0) player.CharityTurnsRemaining--;
        if (player.DownsizedTurnsRemaining > 0) player.DownsizedTurnsRemaining--;
        CycleTurn(game, player);
        gameCache.Set(game.Code, game);
    }

    private int getNewSpaceId(int currentSpaceId, int spacesToMove)
    {
        int combo = currentSpaceId + spacesToMove;
        if (combo > 24)
        {
            return combo - 24;
        }
        return combo;
    }

    private void CycleTurn(Game game, Player player)
    {
        int playerIndex = game.Players.IndexOf(player);
        if (playerIndex == game.Players.Count) playerIndex = 0;
        game.CurrentPlayerId = game.Players[playerIndex].Id;
        game.ConfirmAction = null;
        game.DealAction = null;
        game.MarketAction = null;
        game.CharityAction = null;
    }
}