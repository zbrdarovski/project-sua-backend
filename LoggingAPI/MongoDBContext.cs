using MongoDB.Driver;
using LoggingAPI.Models;

public class MongoDbContext
{
    public IMongoCollection<LoggingEntry> Logs { get; }

    public MongoDbContext(IConfiguration configuration)
    {
        var client = new MongoClient(configuration.GetConnectionString("MongoDBConnection"));
        var database = client.GetDatabase("logs");

        Logs = database.GetCollection<LoggingEntry>("logs");
    }
}
