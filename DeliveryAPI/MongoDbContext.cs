using MongoDB.Driver;
using System;

public class MongoDbContext
{
    public IMongoCollection<Delivery> Deliveries { get; }

    public MongoDbContext(IConfiguration configuration)
    {
        string? environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
        var mongoDbConnectionString = configuration.GetValue<string>("MONGODB_CONNECTION_STRING");
        if (environment == "Development")
        {
            // In Development, use the connection string from appsettings.json
            mongoDbConnectionString = configuration.GetConnectionString("MongoDBConnection");
        }
        else
        {
            // In non-Development, use the environment variable
            mongoDbConnectionString = Environment.GetEnvironmentVariable("MONGODB_CONNECTION_STRING") ?? "mongodb+srv://sua-user:30SD8YKo4tg7R7v5@cluster0.550s6o6.mongodb.net/?retryWrites=true&w=majority";
        }
        var client = new MongoClient(configuration.GetConnectionString("MongoDBConnection"));
        var database = client.GetDatabase("delivery");

        Deliveries = database.GetCollection<Delivery>("delivery");
    }
}
