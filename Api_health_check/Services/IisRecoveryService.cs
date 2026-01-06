using Microsoft.Web.Administration;
using System.Security;

namespace Api_health_check.Services
{
    public class IisRecoveryService
    {
                        //App must run under IIS user with recycle permission
                        //Never expose this via public API
        public bool RestartAppPool(string appPoolName)
        {
            try
            {
                using var server = new ServerManager();
                var pool = server.ApplicationPools[appPoolName];

                if (pool == null) return false;

                pool.Recycle();
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
