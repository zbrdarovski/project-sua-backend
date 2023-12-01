// MongoDbContext.cs
using MongoDB.Driver;

public class MongoDbContext
{
    private readonly IMongoDatabase _database;

    public MongoDbContext(string connectionString, string databaseName)
    {
        if (connectionString is null)
        {
            throw new ArgumentNullException(nameof(connectionString));
        }

        if (databaseName is null)
        {
            throw new ArgumentNullException(nameof(databaseName));
        }

        var client = new MongoClient(connectionString);
        _database = client.GetDatabase(databaseName);
    }

    public IMongoCollection<User> Users => _database.GetCollection<User>("users");
}