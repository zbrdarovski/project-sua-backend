using System.Threading.Tasks;
using System.Collections.Generic;
using StatzAPI.Models;

namespace StatzAPI.Repositories
{
    public interface IStatsRepository
    {
        Task<List<StatsData>> GetAllStats();
        Task<StatsData> GetStatsByEndpoint(string endpoint);
        Task IncrementCallCount(string endpoint);
        Task InsertStatsData(StatsData statsData);
    }
}
