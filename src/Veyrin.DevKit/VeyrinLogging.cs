using Microsoft.Extensions.Logging;

namespace Veyrin.Core.Logging;

public static class VeyrinLogging
{
    /// <summary>
    /// 提供一個內建的預設產生器
    /// </summary>
    /// <param name="level"></param>
    /// <returns></returns>
    private static ILoggerFactory CreateDefaultConsoleFactory(LogLevel level) 
        => LoggerFactory.Create(builder =>{ builder.AddConsole().SetMinimumLevel(level);});

    /// <summary>
    /// 直接調用產生器並初始化 AppLog
    /// </summary>
    public static void SetupConsole(LogLevel minLevel = LogLevel.Information)
        => AppLogger.Init(CreateDefaultConsoleFactory(minLevel));

    /// <summary>
    /// 支援 NLog/Log4Net。<br></br>
    /// 使用者在外部配置好 builder 後傳入。
    /// </summary>
    /// <remarks>
    /// 滿足功能 4
    /// VeyrinLogging.SetupCustom(builder => {
    /// builder.AddNLog();
    /// builder.SetMinimumLevel(LogLevel.Debug);
    /// });
    /// </remarks>
    public static void SetupCustom(Action<ILoggingBuilder> configure)
       => AppLogger.Init(LoggerFactory.Create(configure));
    



}



    ///// <summary>
    ///// [非 DI 環境專用] 
    ///// 自動建立並初始化 Veyrin 全域日誌，預設輸出至 Console。
    ///// 此方法會自動處理 LoggerFactory 的建立與資源管理。
    ///// </summary>
    ///// <param name="configure">額外的日誌配置邏輯</param>
    //public static void SetupConsole(Action<ILoggingBuilder>? configure = null)
    //{
    //    // 建立一個長效型的工廠
    //    var factory = LoggerFactory.Create(builder =>
    //    {
    //        builder.AddConsole();
    //        builder.SetMinimumLevel(LogLevel.Information);
    //        configure?.Invoke(builder);
    //    });

    //    AppLogger.Init(factory);
    //    AppLogger.Any.LogInformation("Veyrin AppLogger 啟動成功：Console 模式");
    //}
