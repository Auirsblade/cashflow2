using Cashflow.API;
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
                      });
    options.AddPolicy("local",
                      policy =>
                      {
                          policy.SetIsOriginAllowedToAllowWildcardSubdomains();
                          policy.WithOrigins("https://localhost:5173");
                      });
});

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

app.UseHttpsRedirection();

app.MapGet("/game/new",
           () =>
           {
               return new Game();
           })
   .WithName("GetNewGame");

app.Run();