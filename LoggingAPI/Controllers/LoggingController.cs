using LoggingAPI.Models;
using Microsoft.AspNetCore.Mvc;

namespace LoggingAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class LoggingController : ControllerBase
    {
       
        private readonly ILogger<LoggingController> _logger;
        private readonly RabbitMQService _rabbitMQService;

        public LoggingController(ILogger<LoggingController> logger, RabbitMQService rabbitMQService)
        {
            _logger = logger;
            _rabbitMQService = rabbitMQService;
        }

        [HttpPost("postMessage")]
        public IActionResult SendMessage([FromBody] string logMessage)
        {
            _rabbitMQService.SendMessage(logMessage);
            return Ok("Log poslan v RabbitMQ.");
        }

        [HttpPost("sendLog")]
        public IActionResult SendLog([FromBody] LoggingEntry logEntry)
        {
            _rabbitMQService.SendLog(logEntry);
            return Ok("Log sent to RabbitMQ.");
        }

        [HttpGet("logs/{startDate}/{endDate}")]
        public IActionResult GetLogs(DateTime startDate, DateTime endDate)
        {
            var logs = _rabbitMQService.GetLogs(startDate, endDate);
            return Ok(logs);
        }

        [HttpDelete("clearLogs")]
        public IActionResult ClearLogs()
        {
            _rabbitMQService.ClearQueue("soa_rv1_upp3");
            return Ok("Logs cleared from the queue.");
        }

    }
}