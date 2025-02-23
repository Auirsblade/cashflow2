using Microsoft.Extensions.Caching.Memory;

namespace Cashflow.API;

public class GameService
{
    private readonly IMemoryCache _gameCache;
    public GameService(IMemoryCache gameCache)
    {
        _gameCache = gameCache;
    }
}