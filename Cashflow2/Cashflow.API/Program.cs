using Cashflow.API;

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
                          policy.WithOrigins("http://localhost:5173", "http://*.ashercarlow.com", "https://*.ashercarlow.com");
                      });
});

var app = builder.Build();
app.UseCors("ashercarlow.com");

// if (app.Environment.IsDevelopment())
// {
    app.MapOpenApi();
// }

app.UseHttpsRedirection();

app.MapGet("/game/new",
           () =>
           {
               var gameCode = Utility.RandomAlphanumericString(4);
               return gameCode;
           })
   .WithName("GetNewGame");

app.Run();