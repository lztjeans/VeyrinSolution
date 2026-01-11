A.Web / DI 專案(整合 Serilog 或 NLog)
// 在 Program.cs 初始化
var app = builder.Build();
LogHelper.SetProvider(new MicrosoftLoggerAdapter(app.Services.GetRequiredService<ILoggerFactory>()));
return app;


B.Unit Test 專案 (使用記憶體或 Console 實作)
public class TestLogger : ILogProvider
{
    public List<string> Logs = new List<string>();
    public void Log(LogLevel level, string message, Exception ex, params object[] args)
    {
        Logs.Add($"[{level}] {message}");
    }
}

// 測試初始化
LogHelper.SetProvider(new TestLogger());
// 執行測試...
Assert.Contains(testLogger.Logs, x => x.Contains("預期的錯誤訊息"));

C. Console 專案 (極簡實作)
LogHelper.SetProvider(new ConsoleProvider()); // 外部實作的簡單 Provider
LogHelper.Info("程式啟動...");
////////////////////////////////////////////////////////////////////////////////////////////////////////

2. 在 Program.cs 進行配置
// 1. NLog 初期化
var logger = NLog.LogManager.Setup().LoadConfigurationFromAppSettings().GetCurrentClassLogger();

try
{
    var builder = WebApplication.CreateBuilder(args);

    // 2. 整合 NLog 到 .NET Logging 系統
    builder.Logging.ClearProviders();
    builder.Host.UseNLog();

    var app = builder.Build();

    // 3. 重點：將 DI 容器中的 LoggerFactory 賦值給靜態類別
    AppLogger.Configure(app.Services.GetRequiredService<ILoggerFactory>());

    app.Run();
}
catch (Exception exception)
{
    logger.Error(exception, "Stopped program because of exception");
    throw;
}
finally
{
    NLog.LogManager.Shutdown();
}
////////////////////////////////////////////////////////////////////////////////////////////////////////
3. 在應用程式中使用
public class MyBusinessService
{
    // 方式 A：定義類別層級的 logger
    private static readonly ILogger _log = AppLogger.CreateLogger<MyBusinessService>();

    public void DoWork()
    {
        _log.LogInformation("正在執行業務邏輯...");
    }
}