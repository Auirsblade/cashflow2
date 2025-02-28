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
        game.Players.First(x => x.Id == player.Id).SetProfession(profession);
        gameCache.Set(game.Code, game);
    }
}