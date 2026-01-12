using Api_health_check.Data;

public interface IRecoveryClient
{
    Task<bool> TriggerRecoveryAsync(MonitoredEndpoint endpoint);
}
