using Cashflow.API.Entities;
using Microsoft.Extensions.Caching.Memory;

namespace Cashflow.API.Services;

public class GameService(IMemoryCache gameCache)
{
    public Game CreateGame(Player creator)
    {
        Game game = new();
        game.Players.Add(creator);

        gameCache.Set(game.Code, game);

        return game;
    }

    public Game? JoinGame(Player player, string gameCode)
    {
        Game? game = gameCache.Get<Game>(gameCode);

        if (game == null) return null;

        game.Players.Add(player);
        gameCache.Set(game.Code, game);
        return game;
    }
}