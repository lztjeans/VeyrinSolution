using Microsoft.Extensions.Logging;

namespace Veyrin.Extend.Policy;

public interface IRetryPolicy
{
    Task<T> ExecuteAsync<T>(Func<Task<T>> action, ILogger logger, string context);
}
