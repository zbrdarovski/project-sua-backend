using System.Threading.Tasks;
using System.Collections.Generic;
using StatzAPI.Models;
using MongoDB.Driver;

namespace StatzAPI.Repositories
{
    public class StatsRepository : IStatsRepository
    {
        private readonly IMongoDBContext _context;

        public StatsRepository(IMongoDBContext context)
        {
            _context = context;
        }

        public async Task<List<StatsData>> GetAllStats()
        {
            return await _context.StatsCollection.Find(_ => true).ToListAsync();
        }

        public async Task<StatsData> GetStatsByEndpoint(string endpoint)
        {
            return await _context.StatsCollection.Find(s => s.Endpoint == endpoint).FirstOrDefaultAsync();
        }

        public async Task IncrementCallCount(string endpoint)
        {
            var filter = Builders<StatsData>.Filter.Eq(s => s.Endpoint, endpoint);
            var update = Builders<StatsData>.Update.Inc(s => s.CallCount, 1);
            await _context.StatsCollection.UpdateOneAsync(filter, update);
        }

        public async Task InsertStatsData(StatsData statsData)
        {
            await _context.StatsCollection.InsertOneAsync(statsData);
        }
    }
}
