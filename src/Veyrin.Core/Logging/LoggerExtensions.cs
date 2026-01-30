
using Microsoft.Extensions.Logging;

namespace Veyrin.Core.Logging;

public static class LoggerExtensions
{
	/// <summary>
	/// [非 DI 環境] 快速初始化 AppLogger 並綁定至控制台輸出
	/// 適用於：Console App, 簡單工具腳本
	/// </summary>
	//public static void SyncToVeyrin(this ILoggerFactory factory)
	//{
	//	AppLogger.Init(factory);
	//	AppLogger.Any.LogInformation("Veyrin AppLogger initialized using Console Provider.");
	//}

	/// <summary>
	/// [DI 環境] 在 Host 建立後快速對接 AppLogger
	/// 適用於：ASP.NET Core, Generic Host
	/// </summary>
	public static void SyncToVeyrin(this IServiceProvider serviceProvider)
	{
        if (serviceProvider.GetService(typeof(ILoggerFactory)) is ILoggerFactory factory)
            AppLogger.Init(factory);
    }
}

//using Microsoft.Extensions.Logging;
//namespace Veyrin.Core.Logging
//{
//    public static class LoggerHelperExtensions
//    {
//        public static void LogInfoWithModule(this ILogger logger, string module, string message)
//        {
//            logger.LogInformation("[{Module}] {Message}", module, message);
//        }
//        public static void LogErrorWithModule(this ILogger logger, string module, string message, Exception? ex = null)
//        {
//            logger.LogError(ex, "[{Module}] {Message}", module, message);
//        }
//        public static void LogDebugWithModule(this ILogger logger, string module, string message)
//        {
//            logger.LogDebug("[{Module}] {Message}", module, message);
//        }
//    }
//}
