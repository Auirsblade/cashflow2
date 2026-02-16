using System.Text.Json;
using Cashflow.API.Entities;
using Cashflow.API.Resources;
using Microsoft.Extensions.Caching.Memory;

namespace Cashflow.API.Services;

public class GameService(IMemoryCache gameCache)
{
    public Game? GetGame(string gameCode) => gameCache.Get<Game>(gameCode.ToUpper());

    public void UpdateGame(Game game) => gameCache.Set(game.Code, game);

    public Game CreateGame(Player creator)
    {
        Game game = new();
        game.StockMarket = StockMarketService.InitializeMarket();
        game.Players.Add(creator);
        game.CurrentPlayerId = creator.Id;
        game.CreatorId = creator.Id;

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
                var marketOffer = MarketGenerator.GenerateAssetOffer();
                var marketAction = new MarketAction(marketOffer);
                // Auto-pass active players who don't own assets of this type
                foreach (var p in game.Players.Where(p => p.IsActive && p.Id != player.Id))
                {
                    if (!p.Assets.Any(a => a.Type == marketOffer.Type))
                    {
                        marketAction.PlayersResponded.Add(p.Id);
                    }
                }
                game.MarketAction = marketAction;
                // If turn player also has no assets of this type, just set the action but don't auto-pass them
                // (frontend will show read-only card with auto-pass timer)
                break;
            case "doodad":
                List<Doodad> doodads = JsonSerializer.Deserialize<List<Doodad>>(File.ReadAllText(@"./Resources/Doodads.json")) ?? [];
                Doodad doodad = doodads[Random.Shared.Next(0, doodads.Count)];
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

    public void BuyDealWithLoan(Game game, Player player, int loanTerm)
    {
        Asset? deal = game.DealAction?.Asset;
        if (deal == null) return;

        decimal shortfall = deal.Equity - player.Cash;
        if (shortfall <= 0)
        {
            BuyDeal(game, player);
            return;
        }

        bool loanSuccess = LoanService.TakeOutLoan(player, shortfall, loanTerm, "Deal Loan");
        if (!loanSuccess) return;

        player.BuyAsset(deal);
        CycleTurn(game, player);
        gameCache.Set(game.Code, game);
    }

    public void SellDeal(Game game, Player player)
    {
        if (game.DealAction?.Asset == null) return;
        if (game.DealAction.AuctionState != null) return;
        if (game.Players.Count(p => p.IsActive) < 2) return;

        game.DealAction.AuctionState = new AuctionState { SellerId = player.Id };
        gameCache.Set(game.Code, game);
    }

    public void PlaceBid(Game game, Player player, decimal bidAmount)
    {
        AuctionState? auction = game.DealAction?.AuctionState;
        if (auction == null || auction.IsComplete) return;
        if (player.Id == auction.SellerId) return;
        if (auction.Bids.ContainsKey(player.Id)) return;
        if (bidAmount <= 0) return;

        Asset deal = game.DealAction!.Asset!;
        if (bidAmount + deal.Equity > player.Cash) return;

        auction.Bids[player.Id] = bidAmount;
        CheckAuctionComplete(game);
        gameCache.Set(game.Code, game);
    }

    public void AuctionPass(Game game, Player player)
    {
        AuctionState? auction = game.DealAction?.AuctionState;
        if (auction == null || auction.IsComplete) return;
        if (player.Id == auction.SellerId) return;
        if (auction.Bids.ContainsKey(player.Id)) return;

        auction.Bids[player.Id] = null;
        CheckAuctionComplete(game);
        gameCache.Set(game.Code, game);
    }

    private void CheckAuctionComplete(Game game)
    {
        AuctionState auction = game.DealAction!.AuctionState!;
        int otherPlayerCount = game.Players.Count(p => p.IsActive && p.Id != auction.SellerId);
        if (auction.Bids.Count < otherPlayerCount) return;

        auction.IsComplete = true;

        int sellerIndex = game.Players.FindIndex(p => p.Id == auction.SellerId);
        decimal highestBid = 0;
        Guid? winnerId = null;

        for (int i = 1; i < game.Players.Count; i++)
        {
            int idx = (sellerIndex + i) % game.Players.Count;
            Player p = game.Players[idx];
            if (auction.Bids.TryGetValue(p.Id, out decimal? bid) && bid.HasValue && bid.Value > highestBid)
            {
                highestBid = bid.Value;
                winnerId = p.Id;
            }
        }

        if (winnerId != null)
        {
            Player winner = game.Players.First(p => p.Id == winnerId);
            Player seller = game.Players.First(p => p.Id == auction.SellerId);
            Asset deal = game.DealAction!.Asset!;

            winner.BuyAsset(deal);
            winner.Cash -= highestBid;
            seller.Cash += highestBid;

            auction.WinnerId = winnerId;
            auction.WinningBid = highestBid;
        }
    }

    public void SellToMarket(Game game, Player player, Asset asset)
    {
        if (game.MarketAction == null) return;
        if (!game.MarketAction.PlayersResponded.Add(player.Id)) return;
        player.SellAsset(game.MarketAction.PurchaseOffer, asset);
        if (game.MarketAction.PlayersResponded.Count >= game.Players.Count(p => p.IsActive)) CycleTurn(game, game.Players.First(x => x.Id == game.CurrentPlayerId));
        gameCache.Set(game.Code, game);
    }

    public void MarketPass(Game game, Player player)
    {
        if (game.MarketAction == null) return;
        if (!game.MarketAction.PlayersResponded.Add(player.Id)) return;
        if (game.MarketAction.PlayersResponded.Count >= game.Players.Count(p => p.IsActive)) CycleTurn(game, game.Players.First(x => x.Id == game.CurrentPlayerId));
        gameCache.Set(game.Code, game);
    }

    public void EndTurn(Game game, Player player)
    {
        CycleTurn(game, player);
        gameCache.Set(game.Code, game);
    }

    public void PayDoodad(Game game, Player player, bool useCard)
    {
        Doodad? doodad = game.ConfirmAction?.Doodad;
        if (doodad == null) return;

        if (useCard)
        {
            Liability? card = player.Liabilities.FirstOrDefault(l => l.Term <= 0);
            if (card != null)
            {
                card.Amount += doodad.Cost;
            }
            else
            {
                player.Liabilities.Add(new Liability
                {
                    Name = "Credit Card",
                    Amount = doodad.Cost,
                    InterestRate = 0.18m,
                    Term = FinancialConstants.CREDIT_CARD_TERM
                });
            }
        }
        else
        {
            if (player.Cash < doodad.Cost) return;
            player.Cash -= doodad.Cost;
        }

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

    public void BuyStock(Game game, Player player, string ticker, int quantity)
    {
        StockMarketService.BuyStock(game, player, ticker, quantity);
        gameCache.Set(game.Code, game);
    }

    public void SellStock(Game game, Player player, string ticker, int quantity)
    {
        StockMarketService.SellStock(game, player, ticker, quantity);
        gameCache.Set(game.Code, game);
    }

    public void TakeOutLoan(Game game, Player player, decimal amount, int term)
    {
        LoanService.TakeOutLoan(player, amount, term);
        gameCache.Set(game.Code, game);
    }

    public void PayOffLoan(Game game, Player player, Guid liabilityId, decimal amount)
    {
        LoanService.PayOffLoan(player, liabilityId, amount);
        gameCache.Set(game.Code, game);
    }

    public void RemovePlayer(Game game, Player player)
    {
        player.IsActive = false;

        // Auto-pass from pending auction
        AuctionState? auction = game.DealAction?.AuctionState;
        if (auction != null && !auction.IsComplete && player.Id != auction.SellerId && !auction.Bids.ContainsKey(player.Id))
        {
            auction.Bids[player.Id] = null;
            CheckAuctionComplete(game);
        }

        // Auto-pass from pending market
        if (game.MarketAction != null)
        {
            game.MarketAction.PlayersResponded.Add(player.Id);
            if (game.MarketAction.PlayersResponded.Count >= game.Players.Count(p => p.IsActive))
                CycleTurn(game, game.Players.First(x => x.Id == game.CurrentPlayerId));
        }

        // If it's their turn, cycle to next
        if (game.CurrentPlayerId == player.Id)
        {
            CycleTurn(game, player);
        }

        gameCache.Set(game.Code, game);
    }

    private void CycleTurn(Game game, Player player)
    {
        if (player.CharityTurnsRemaining > 0) player.CharityTurnsRemaining--;
        if (player.DownsizedTurnsRemaining > 0) player.DownsizedTurnsRemaining--;

        int playerIndex = game.Players.IndexOf(player);
        int count = game.Players.Count;
        for (int i = 1; i <= count; i++)
        {
            int idx = (playerIndex + i) % count;
            if (game.Players[idx].IsActive)
            {
                game.CurrentPlayerId = game.Players[idx].Id;
                break;
            }
        }
        game.ConfirmAction = null;
        game.DealAction = null;
        game.MarketAction = null;
        game.CharityAction = null;

        StockMarketService.UpdatePrices(game.StockMarket);
        StockMarketService.RecalculateAllDividends(game);
        game.StockMarket.TurnNumber++;
    }
}