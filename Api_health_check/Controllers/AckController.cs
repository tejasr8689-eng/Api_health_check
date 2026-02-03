using Api_health_check.Models;
using Api_health_check.Data;
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

        private readonly HealthCheckContext _context;
        public AckController(ILogger<AckController> logger,HealthCheckContext context)
        {
            _logger = logger;
            _context = context;
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
                payload.target,
                payload.action,
                payload.status,
                payload.details,
                payload.completedAt);
           
            _context.RecoveryAcks.Add(new RecoveryAck
            {
                requestId = finalRequestId,
                target = payload.target,
                status = payload.status,
                details = payload.details,
                completedAt = payload.completedAt,
                receivedAt = DateTime.UtcNow
            });
            _context.SaveChangesAsync();
            return Ok(new { received = true, requestId });

        }
    }
}
