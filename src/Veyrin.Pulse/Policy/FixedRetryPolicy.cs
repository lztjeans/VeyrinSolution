using Microsoft.Extensions.Logging;

namespace Veyrin.Extend.Policy;

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
        Exception? lastException = null;

        for (int attempt = 1; attempt <= _maxRetries; attempt++)
        {
            try
            {
                logger.LogInformation("[{Context}] Attempt {Attempt}/{Max}", context, attempt, _maxRetries);
                return await action();
            }
            catch (Exception ex)
            {
                lastException = ex;
                if (attempt < _maxRetries)
                {
                    logger.LogWarning(ex, "[{Context}] Attempt {Attempt} failed, retrying in {_delay}...", context, attempt, _delay);
                    await Task.Delay(_delay);
                }
            }
        }

        // 拋出包含上下文的 Veyrin 異常
        throw new HttpRequestException($"[{context}] Operation failed after {_maxRetries} attempts. Last Error: {lastException?.Message}", lastException);
    }
}
