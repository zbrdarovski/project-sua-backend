using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using StatzAPI.Models;
using StatzAPI.Repositories;
using StatzAPI.Services;

namespace StatzAPI.Services
{
    public class StatsService : IStatsService
    {
        private readonly IStatsRepository _repository;

        public StatsService(IStatsRepository repository)
        {
            _repository = repository;
        }

        public async Task<StatsData> GetLastCalledEndpoint()
        {
            var statsList = await _repository.GetAllStats();
            return statsList.OrderByDescending(s => s.LastAccessed).FirstOrDefault();
        }

        public async Task<StatsData> GetMostFrequentlyCalledEndpoint()
        {
            var statsList = await _repository.GetAllStats();
            return statsList.OrderByDescending(s => s.CallCount).FirstOrDefault();
        }

        public async Task<Dictionary<string, int>> GetCallsPerEndpoint()
        {
            var statsList = await _repository.GetAllStats();
            return statsList.ToDictionary(s => s.Endpoint, s => s.CallCount);
        }

        public async Task UpdateStatsData(string endpoint)
        {
            var statsData = await _repository.GetStatsByEndpoint(endpoint);
            if (statsData == null)
            {
                statsData = new StatsData { Endpoint = endpoint, CallCount = 1, LastAccessed = DateTime.Now };
                await _repository.InsertStatsData(statsData);
            }
            else
            {
                await _repository.IncrementCallCount(endpoint);
            }
        }
    }
}
