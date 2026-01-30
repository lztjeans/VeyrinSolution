using Microsoft.Extensions.Logging;

namespace Veyrin.Extend.Policy;

public interface IRetryPolicy
{
    Task<T> ExecuteAsync<T>(Func<Task<T>> action, ILogger logger, string context);
}

public class FixedRetryPolicy : IRetryPolicy
{
    private readonly int _maxRetries;
    private readonly TimeSpan _delay;

    public FixedRetryPolicy(int maxRetries = 3, TimeSpan? delay = null)
    {
        _maxRetries = maxRetries;
        _delay = delay ?? TimeSpan.FromSeconds(2);
    }

    public async Task<T> ExecuteAsync<T>(Func<Task<T>> action, ILogger logger, string context)
    {
        for (int attempt = 1; attempt <= _maxRetries; attempt++)
        {
            try
            {
                logger.LogInformation("[{Context}] Attempt {Attempt}", context, attempt);
                return await action();
            }
            catch (Exception ex) when (attempt < _maxRetries)
            {
                logger.LogWarning(ex, "[{Context}] Failed attempt {Attempt}, retrying...", context, attempt);
                await Task.Delay(_delay);
            }
        }

        throw new Exception($"[{context}] Failed after {_maxRetries} attempts");
    }
}
