using MongoDB.Driver;

namespace StatzAPI.Models
{
    public class MongoDBContext : IMongoDBContext
    {
        private readonly IMongoDatabase _database;

        public MongoDBContext(MongoDBSettings settings)
        {
            var client = new MongoClient(settings.ConnectionString);
            _database = client.GetDatabase(settings.DatabaseName);
        }

        public IMongoCollection<StatsData> StatsCollection => _database.GetCollection<StatsData>("stats");
    }

    public interface IMongoDBContext
    {
        IMongoCollection<StatsData> StatsCollection { get; }
    }
}
