using Api_health_check.Models;
using Azure.Core;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Api_health_check.Controllers
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

        [HttpPost("{requestId}")]
        public IActionResult ReceiveAck(string requestId, [FromBody] RecoveryAck payload)
        { 
            if (payload == null)
            {
                return BadRequest("Payload is null");
            }
            var finalRequestId = requestId ?? payload.requestId;

            _logger.LogInformation(
                "ACK received | RequestId: {requestId} | Target: {target} | Action: {action} | Status: {status} | Details: {details} | CompletedAt: {completedAt}",
                finalRequestId, 
                finalRequestId, 
                payload.target,
                payload.action,
                payload.status,
                payload.details,
                payload.completedAt);
            return Ok(new {received = true , requestId});

        }
    }
}
