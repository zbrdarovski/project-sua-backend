using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using StatzAPI.Models;
using StatzAPI.Services;

namespace StatzAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class StatsController : ControllerBase
    {
        private readonly IStatsService _statsService;

        public StatsController(IStatsService statsService)
        {
            _statsService = statsService;
        }

        [HttpGet("last-called")]
        public async Task<ActionResult<StatsData>> GetLastCalledEndpoint()
        {
            var lastCalledEndpoint = await _statsService.GetLastCalledEndpoint();
            if (lastCalledEndpoint == null)
                return NotFound();
            return Ok(lastCalledEndpoint);
        }

        [HttpGet("most-frequent")]
        public async Task<ActionResult<StatsData>> GetMostFrequentlyCalledEndpoint()
        {
            var mostFrequentEndpoint = await _statsService.GetMostFrequentlyCalledEndpoint();
            if (mostFrequentEndpoint == null)
                return NotFound();
            return Ok(mostFrequentEndpoint);
        }

        [HttpGet("calls-per-endpoint")]
        public async Task<ActionResult<Dictionary<string, int>>> GetCallsPerEndpoint()
        {
            var callsPerEndpoint = await _statsService.GetCallsPerEndpoint();
            return Ok(callsPerEndpoint);
        }

        [HttpPost("update-data")]
        public async Task<IActionResult> UpdateData([FromBody] UpdateDataRequest request)
        {
            if (request == null || string.IsNullOrWhiteSpace(request.KlicanaStoritev))
                return BadRequest("Invalid request");

            await _statsService.UpdateStatsData(request.KlicanaStoritev);
            return Ok();
        }
    }
}
