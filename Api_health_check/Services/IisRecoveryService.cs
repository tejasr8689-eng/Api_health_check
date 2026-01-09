using System;
using Microsoft.Extensions.Logging;
using Microsoft.Web.Administration;

namespace Api_health_check.Services
{
    public class IisRecoveryService
    {
        private readonly ILogger<IisRecoveryService> _logger;
        private string? _lastError;

        public IisRecoveryService(ILogger<IisRecoveryService> logger)
        {
            _logger = logger;
        }

        public string? LastError => _lastError;

        public bool RestartAppPool(string appPoolName)
        {
            _lastError = null;

            if (string.IsNullOrWhiteSpace(appPoolName))
            {
                _lastError = "AppPoolName is empty.";
                _logger.LogWarning(_lastError);
                return false;
            }

            try
            {
                _logger.LogDebug("Attempting to recycle app pool '{AppPoolName}'", appPoolName);

                using var server = new ServerManager();
                var pool = server.ApplicationPools[appPoolName];

                if (pool == null)
                {
                    _lastError = $"Application pool '{appPoolName}' not found.";
                    _logger.LogWarning(_lastError);
                    return false;
                }

                pool.Recycle();
                _logger.LogInformation("Successfully recycled app pool '{AppPoolName}'.", appPoolName);
                return true;
            }
            catch (UnauthorizedAccessException uex)
            {
                _lastError = "Permission denied: process identity cannot manage IIS.";
                _logger.LogError(uex, _lastError + " AppPool: {AppPoolName}", appPoolName);
                return false;
            }
            catch (Exception ex)
            {
                _lastError = ex.Message;
                _logger.LogError(ex, "Unexpected error recycling app pool '{AppPoolName}'", appPoolName);
                return false;
            }
        }
    }
}
