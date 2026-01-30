using Microsoft.Extensions.Logging;
using Veyrin.Core.Interfaces;
using Veyrin.Core.Models;

namespace Veyrin.Core.Adapter;

// 在 Web 專案中實作適配器
public class MicrosoftLoggerAdapter : ILogProvider
{
    private readonly ILogger _logger;
    public MicrosoftLoggerAdapter(ILoggerFactory factory) => _logger = factory.CreateLogger("Global");

    public void Log(LogLevelEnum level, string message, Exception? ex, params object[] args)
    {
        // 這裡將自定義的 LogLevel 對應到 Microsoft.Extensions.Logging.LogLevel
        var msLevel = level switch
        {
            LogLevelEnum.Trace => LogLevel.Trace,
            LogLevelEnum.Debug => LogLevel.Debug,
            LogLevelEnum.Info => LogLevel.Information,
            LogLevelEnum.Warn => LogLevel.Warning,
            LogLevelEnum.Error => LogLevel.Error,
            LogLevelEnum.Fatal => LogLevel.Critical,
            _ => LogLevel.None
        };

        _logger.Log(msLevel, ex, message, args);
    }

}
