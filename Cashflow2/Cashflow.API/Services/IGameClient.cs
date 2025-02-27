using Cashflow.API.Entities;
using SignalRSwaggerGen.Attributes;

namespace Cashflow.API.Services;

[SignalRHub]
public interface IGameClient
{
    Task GameStateUpdated(Game game);
    Task Error(string message);
}