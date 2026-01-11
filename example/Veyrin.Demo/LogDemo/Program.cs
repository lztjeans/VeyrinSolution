using Microsoft.Extensions.Logging;
using NLog.Extensions.Logging;
using Veyrin.Core.Logging;

namespace Veyrin.Demo.LogDemo;

public class Program
{
    public static void Main(string[] args)
    {
        NewMethod1();
        NewMethod();
        VeyrinLogging.SetupConsole();
        new MyTool().Run();

    }

    private static void NewMethod1()
    {
        // 2. 注入到你的統一入口
        VeyrinLogging.SetupConsole();

        // 3. 測試使用
        var log = AppLogger.Any;
        log.LogInformation("🚀 系統已啟動");

        try
        {
            // 執行你的業務邏輯
            throw new Exception("測試錯誤");
        }
        catch (Exception ex)
        {
            log.LogError(ex, "❌ 發生未預期錯誤");
        }
        new MyTool().Run();
        Console.WriteLine("*******************************");
        Console.WriteLine("*******************************");
        Console.WriteLine("*******************************");
        // Console App 結束前建議呼叫，確保 Log 寫入檔案
        //NLog.LogManager.Shutdown();
    }

    private static void NewMethod()
    {
        // 建立橋接 (這是唯一跟 NLog 有關的地方)
        VeyrinLogging.SetupCustom(builder =>
        {
            builder.AddNLog(); // 它會自動去讀取 nlog.config
            //builder.AddLog4Net();// log4net.config
            builder.SetMinimumLevel(LogLevel.Debug);
        });
        new MyTool().Run();
    }
}

public class MyTool
{
    // 方式 A：企業標準 (類別專屬 Logger)
    private readonly ILogger _log = AppLogger.For<MyTool>();

    public void Run()
    {
        _log.LogInformation("工具開始運作...");
        //Console.WriteLine("工具開始運作1...");

        // 方式 B：臨時呼叫 (Static)
        AppLogger.Any.LogWarning("發生警告，但不影響流程");
    }
}