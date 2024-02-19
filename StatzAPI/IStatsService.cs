using System.Threading.Tasks;
using System.Collections.Generic;
using StatzAPI.Models;

namespace StatzAPI.Services
{
    public interface IStatsService
    {
        Task<StatsData> GetLastCalledEndpoint();
        Task<StatsData> GetMostFrequentlyCalledEndpoint();
        Task<Dictionary<string, int>> GetCallsPerEndpoint();
        Task UpdateStatsData(string endpoint);
    }
}
