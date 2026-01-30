using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace Veyrin.Core.Logging;

public static class AppLogger
{
    private static ILoggerFactory? _factory;
    private static bool _hasWarned = false;
    private static readonly object _lock = new();

    /// <summary>
    /// 初始化 Veyrin 全域日誌工廠。
    /// </summary>
    /// <remarks>
    /// 💡 建議做法：
    /// 1. 非 DI 環境 (Console): 請直接使用 <see cref="VeyrinLogging.SetupConsole"/><br></br>
    /// 2. DI 環境 (Web API): 使用 <c>app.Services.SyncToVeyrin()</c> 擴充方法
    /// <br></br>
    /// 若需手動初始化：<br></br>
    /// <code>
    /// AppLogger.Init(LoggerFactory.Create(b => b.AddConsole()));
    /// </code>
    /// </remarks>
    public static void Init(ILoggerFactory factory) => _factory = factory;
    /// <summary>
    /// 取得安全工廠：若未初始化則回傳 NullLoggerFactory 並給予提示
    /// </summary>
    private static ILoggerFactory SafeFactory
    {
        get
        {
            if (_factory != null) return _factory;

            // 只有在第一次呼叫且未初始化時，透過標準輸出給予提示
            if (!_hasWarned)
            {
                lock (_lock)
                {
                    if (!_hasWarned)
                    {
                        Console.WriteLine("[Veyrin.Core] Warning: AppLogger has not been initialized. Falling back to NullLogger.");
                        Console.WriteLine("[Veyrin.Core] Tip : Call 'AppLogger.Init(loggerFactory)' at application startup.");
                        Console.WriteLine("[Veyrin.Core] Tip : Call 'VeyrinLogging.SetupConsole()' at application startup.");
                        Console.WriteLine("[Veyrin.Core] Tip : Call 'VeyrinLogging.SetupCustom()' at application startup.");
                        _hasWarned = true;
                    }
                }
            }
            return NullLoggerFactory.Instance;
        }
    }
    /// <summary>
    /// 為指定類別建立 Logger
    /// </summary>
    public static ILogger<T> For<T>() => SafeFactory.CreateLogger<T>();
    /// <summary>
    /// 建立全域或指定名稱的 Logger<br></br>
    /// 如果是簡單的腳本，直接用 AppLogger.Any.LogInformation(...) 很方便<br></br>
    /// <param name="categoryName">分類名稱，預設為 Veyrin</param>
    /// </summary>
    public static ILogger Any => SafeFactory.CreateLogger("Veyrin");
}

/*
public static class AppLog2
{
    private static ILoggerFactory? _factory;

    /// <summary>
    /// 使用 Lazy 確保只有在需要「預設 Log」時才建立 Console 工廠
    /// </summary>
    private static readonly Lazy<ILoggerFactory> _defaultFactory = new(() =>
        LoggerFactory.Create(builder => builder.AddConsole().SetMinimumLevel(LogLevel.Information))
    );
    /// <summary>
    /// 初始化 Veyrin 全域日誌工廠
    /// </summary>
    public static void Init(ILoggerFactory factory) => _factory = factory;
    /// <summary>
    /// 取得安全工廠：若未初始化則回傳 DefaultFactory
    /// </summary>
    private static ILoggerFactory SafeFactory => _factory ?? _defaultFactory.Value;
    /// <summary>
    /// 為指定類別建立 Logger
    /// </summary>
    public static ILogger<T> For<T>() => SafeFactory.CreateLogger<T>();
    /// <summary>
    /// 建立全域或指定名稱的 Logger<br></br>
    /// 如果是簡單的腳本，直接用 AppLogger.Any.LogInformation(...) 很方便<br></br>
    /// <param name="categoryName">分類名稱，預設為 Veyrin</param>
    /// </summary>
    public static ILogger Any => SafeFactory.CreateLogger("Veyrin");
}
*/
