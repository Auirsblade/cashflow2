using System.Text.Json;
using Cashflow.API.Entities;
using Cashflow.API.Resources;
using Microsoft.Extensions.Caching.Memory;

namespace Cashflow.API.Services;

public class GameService(IMemoryCache gameCache)
{
    public Game? GetGame(string gameCode) => gameCache.Get<Game>(gameCode.ToUpper());

    public Game CreateGame(Player creator)
    {
        Game game = new();
        game.Players.Add(creator);
        game.CurrentPlayerId = creator.Id;

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
        if (player.DownsizedTurnsRemaining > 0) return;

        int currentSpaceId = player.BoardSpaceId;
        int newSpaceId = getNewSpaceId(currentSpaceId, spacesToMove);
        bool wrapping = newSpaceId <= currentSpaceId;
        bool passedPayday = wrapping
            ? game.BoardSpaces?.Any(x =>
                ((x.Id > currentSpaceId && x.Id <= 24) || (x.Id >= 1 && x.Id < newSpaceId)) &&
                x.Name != null && x.Name.Equals("payday", StringComparison.OrdinalIgnoreCase)) ?? false
            : game.BoardSpaces?.Any(x =>
                x.Id > currentSpaceId && x.Id < newSpaceId &&
                x.Name != null && x.Name.Equals("payday", StringComparison.OrdinalIgnoreCase)) ?? false;
        if (passedPayday)
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
                game.MarketAction = new MarketAction(MarketGenerator.GenerateAssetOffer());
                break;
            case "doodad":
                List<Doodad> doodads = JsonSerializer.Deserialize<List<Doodad>>(File.ReadAllText(@"./Resources/Doodads.json")) ?? [];
                Doodad doodad = doodads[Random.Shared.Next(0, doodads.Count)];
                player.BuyDoodad(doodad);
                game.ConfirmAction = new ConfirmAction(ActionType.Doodad)
                {
                    Doodad = doodad
                };
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
        Asset deal;
        bool goodDeal;
        do
        {
            deal = isBig ? AssetGenerator.GenerateBigDeal() : AssetGenerator.GenerateSmallDeal();

            goodDeal = deal.Type is not (var type and (AssetType.mlm1 or AssetType.mlm2)) || player.Assets.All(x => x.Type != type);
        } while (!goodDeal);

        if (game.DealAction != null) game.DealAction.Asset = deal;

        gameCache.Set(game.Code, game);
    }

    public void BuyDeal(Game game, Player player)
    {
        Asset? deal = game.DealAction?.Asset;
        if (deal == null) return;
        if (deal.Equity > player.Cash) return;

        player.BuyAsset(deal);
        CycleTurn(game, player);
        gameCache.Set(game.Code, game);
    }

    public void SellDeal(Game game, Player player)
    {
        // Not yet implemented — return gracefully instead of crashing
    }

    public void SellToMarket(Game game, Player player, Asset asset)
    {
        if (game.MarketAction == null) return;
        if (!game.MarketAction.PlayersResponded.Add(player.Id)) return;
        player.SellAsset(game.MarketAction.PurchaseOffer, asset);
        if (game.MarketAction.PlayersResponded.Count == game.Players.Count) CycleTurn(game, game.Players.First(x => x.Id == game.CurrentPlayerId));
        gameCache.Set(game.Code, game);
    }

    public void MarketPass(Game game, Player player)
    {
        if (game.MarketAction == null) return;
        if (!game.MarketAction.PlayersResponded.Add(player.Id)) return;
        if (game.MarketAction.PlayersResponded.Count == game.Players.Count) CycleTurn(game, game.Players.First(x => x.Id == game.CurrentPlayerId));
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
        int playerIndex = game.Players.IndexOf(player) + 1;
        if (playerIndex == game.Players.Count) playerIndex = 0;
        game.CurrentPlayerId = game.Players[playerIndex].Id;
        game.ConfirmAction = null;
        game.DealAction = null;
        game.MarketAction = null;
        game.CharityAction = null;
    }
}