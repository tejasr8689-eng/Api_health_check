using Api_health_check.Models;
using Azure.Core;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Api_health_check.Controller
{
    [ApiController]
    [Route("api/[controller]")]
    public class AckController : ControllerBase
    { 
        private readonly ILogger<AckController> _logger;
        public AckController(ILogger<AckController> logger)
        {
            _logger = logger;
        }

        [HttpPost("{RequestId}")]
        public async  Task<IActionResult> ReceiveAck(string RequestId, [FromBody] RecoveryAck payload)
        { 
            _logger.LogInformation("Received acknowledgment for RequestId: {RequestId}, Action: {Action}, Status: {Status}, CompletedAt: {CompletedAt}",
                RequestId ?? payload.RequestId, 
                payload.target,
                payload.Action,
                payload.Details,
                payload.Status,
                payload.CompletedAt);
            return Ok(new {received = true , RequestId});

        }
    }
}
