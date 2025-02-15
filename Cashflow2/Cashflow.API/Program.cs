using Cashflow.API;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

//app.UseHttpsRedirection();

app.MapGet("/game/new",
           () =>
           {
               var gameCode = Utility.RandomAlphanumericString(4);
               return gameCode;
           })
   .WithName("GetNewGame");

app.Run();