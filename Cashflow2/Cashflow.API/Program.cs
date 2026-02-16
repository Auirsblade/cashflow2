using System.Net.WebSockets;
using Cashflow.API;
using Cashflow.API.DTOs;
using Cashflow.API.Entities;
using Cashflow.API.Services;
using Microsoft.OpenApi;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddCors(options =>
{
    options.AddPolicy("ashercarlow.com",
                      policy =>
                      {
                          policy.WithOrigins("https://cf2.ashercarlow.com");
                          policy.AllowAnyHeader();
                          policy.AllowCredentials();
                      });
    options.AddPolicy("local",
                      policy =>
                      {
                          policy.WithOrigins("https://localhost:5173");
                          policy.AllowAnyMethod();
                          policy.AllowAnyHeader();
                          policy.AllowCredentials();
                      });
});

DotEnv.Load(Path.Combine(Directory.GetCurrentDirectory(), ".env"));
Console.Out.WriteLine(Path.Combine(Directory.GetCurrentDirectory(), ".env"));
builder.Configuration.AddEnvironmentVariables();

builder.WebHost.UseUrls(Environment.GetEnvironmentVariable("DOTNET_URLS")!);
builder.Services.AddMemoryCache();
builder.Services.AddScoped<GameService>();
builder.Services.AddSignalR();

builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo { Title = "Cashflow API v1", Version = "v1" });
    // here some other configurations maybe...
    options.AddSignalRSwaggerGen();
});

WebApplication app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.UseCors("local");
}
else
{
    app.UseCors("ashercarlow.com");
}

//app.UseHttpsRedirection();

app.MapHub<GameHub>("/gameHub");

// app.MapPost("/game/new",
//             (GameRequest request) =>
//             {
//                 Game game = new();
//                 game.Players.Add(request.player);
//
//                 return new GameResponse
//                 {
//                     CurrentPlayer = request.player,
//                     game = game
//                 };
//             })
//    .WithName("PostNewGame");

app.Run();