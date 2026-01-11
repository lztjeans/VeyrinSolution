using Microsoft.Extensions.Logging;
using NLog.Extensions.Logging;
using Veyrin.Cli.Commands;
using Veyrin.Cli.Terminal;
using Veyrin.Config.Models;
using Veyrin.Config.Providers;
using Veyrin.Config;
using Veyrin.Pulse;
using Veyrin.Config.Utils;
using Veyrin.Config.Environments;
using Veyrin.Core.Logging;

namespace Veyrin.Demo.SyncTool;

public class SyncTool
{
    public static async Task Main(string[] args) {
        // ---1.初始化 Configuration(對應 'config')-- -
        //// 這會讀取 appsettings.json 並支援環境變數
        //IConfiguration config = new ConfigurationBuilder()
        //    .SetBasePath(Directory.GetCurrentDirectory())
        //    .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
        //    .AddEnvironmentVariables()
        //    .Build();
        string basePath = ConfigPathUtils.CurrentDirectory;
        string env = EnvHelper.Default.EnvironmentName;
        var configFiles = new List<ConfigFileDescriptor>
        {
            new("appsettings.json", ConfigFileType.Json),
            new("customsettings.ini", ConfigFileType.Ini)
        };
        ConfigHelper.Initialize(
            new JsonConfigProvider(basePath, env),
            new EnvironmentVariableProvider(),
            new FileConfigLoader(configFiles)
        );

        //IConfiguration config = ConfigHelper.ge

        // --- 2. 初始化 Logging (對應 'loggerFactory') ---
        // 設定如何輸出 Log（例如輸出到 Console）
        //using ILoggerFactory loggerFactory = LoggerFactory.Create(builder =>
        //{
        //    builder
        //        .AddConfiguration(config.GetSection("Logging"))
        //        .AddConsole(); // 讓 Pulse 的 Retry 訊息能顯示在畫面上
        //});
        using var loggerFactory = LoggerFactory.Create(builder =>
        {
            builder.SetMinimumLevel(LogLevel.Trace);
            //builder.AddConsole();
            builder.AddNLog(); // 它會自動去讀取 nlog.config
            //builder.AddLog4Net();// log4net.config
        });
        AppLogger.Init(loggerFactory);
        var log = AppLogger.For<Program>();

        // 3. 主程式邏輯 (CLI 啟動)
        var executor = new CommandExecutor();

        executor.Register("sync", async (args) =>
        {
            ConsoleWriter.Header("Veyrin Data Synchronizer");

            // 取得 CLI 參數
            string apiUrl = args.GetOption("url") ?? "https://api.veyrin.com/products";
            bool dryRun = args.HasFlag("dry-run");

            // A. 啟動網路請求 (Pulse)
            using var spinner = new Spinner();
            ConsoleWriter.Info($"Fetching data from {apiUrl}...");

            //var pulse = new PulseClient(new HttpClient(), loggerFactory.CreateLogger<PulseClient>());
            var pulse = new PulseClient(logger: AppLogger.For<PulseClient>());
            
            var remoteData = await pulse.GetAsync<List<ProductEntity>>(apiUrl);

            if (remoteData == null || remoteData.Count == 0)
            {
                spinner.Stop("No data found.");
                ConsoleWriter.Error("Sync aborted: Remote source returned empty.");
                return;
            }

            // B. 資料寫入 (Data)
            if (!dryRun)
            {
                ConsoleWriter.Info($"Syncing {remoteData.Count} items to SQL Server...");
                var repo = new ProductRepository(ConfigHelper.GetConfiguration());
                await repo.SyncProductsAsync(remoteData);
            }

            spinner.Stop("Sync Completed!");

            // C. 顯示成果 (Terminal Table)
            TableWriter.Write(remoteData.Take(5), "Synced Preview (First 5 Items)");
            ConsoleWriter.Success($"Successfully processed {remoteData.Count} records.");
        });

        // 啟動點
        await executor.ExecuteAsync(args);
    }
}
