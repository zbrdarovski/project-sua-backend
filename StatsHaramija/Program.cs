using StatsHaramija;
using System;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

string? environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");

builder.Services.AddSingleton<StatsRepository>(serviceProvider =>
{
    string mongoDbConnectionString;

    if (environment == "Development")
    {
        // In Development, use the connection string from appsettings.json
        mongoDbConnectionString = builder.Configuration.GetConnectionString("MongoDBConnection");
    }
    else
    {
        // In non-Development, use the environment variable
        mongoDbConnectionString = Environment.GetEnvironmentVariable("MONGODB_CONNECTION_STRING") ?? "mongodb+srv://sua-user:30SD8YKo4tg7R7v5@cluster0.550s6o6.mongodb.net/?retryWrites=true&w=majority";
    }

    return new StatsRepository(mongoDbConnectionString);
});

var app = builder.Build();


// Configure the HTTP request pipeline.

app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "StatsHaramija");
});

app.MapControllers();

app.Run();
