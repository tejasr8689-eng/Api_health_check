namespace Api_health_check.Services
{
    public class RetryPolicyService
    {
        public async Task<bool> ExecuteAsync(Func<Task<bool>> action, int maxRetries = 3)
        {
            int delayMs = 2000;

            for (int attempt = 1; attempt <= maxRetries; attempt++)
            {
                if (await action())
                    return true;

                await Task.Delay(delayMs);
                delayMs *= 2; // exponential backoff
            }

            return false;
        }
    }

}
