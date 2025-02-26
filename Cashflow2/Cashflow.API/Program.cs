using Cashflow.API;
using Cashflow.API.DTOs;
using Cashflow.API.Entities;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddCors(options =>
{
    options.AddPolicy("ashercarlow.com",
                      policy =>
                      {
                          policy.SetIsOriginAllowedToAllowWildcardSubdomains();
                          policy.WithOrigins("https://*.ashercarlow.com");
                          policy.WithHeaders("Content-Type");
                      });
    options.AddPolicy("local",
                      policy =>
                      {
                          policy.WithOrigins("https://localhost:5173");
                          policy.WithHeaders("Content-Type");
                      });
});

DotEnv.Load(Path.Combine(Directory.GetCurrentDirectory(), ".env"));
Console.Out.WriteLine(Path.Combine(Directory.GetCurrentDirectory(), ".env"));
builder.Configuration.AddEnvironmentVariables();

builder.WebHost.UseUrls(Environment.GetEnvironmentVariable("DOTNET_URLS")!);
builder.Services.AddMemoryCache();
builder.Services.AddScoped<GameService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseCors("local");
}
else
{
    app.UseCors("ashercarlow.com");
}

//app.UseHttpsRedirection();

app.MapPost("/game/new",
            (GameRequest request) =>
            {
                Game game = new();
                game.Players.Add(request.player);

                return new GameResponse
                {
                    CurrentPlayer = request.player,
                    game = game
                };
            })
   .WithName("PostNewGame");

app.Run();