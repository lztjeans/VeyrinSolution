using Veyrin.Config;
using Veyrin.Config.Environments;
using Veyrin.Config.Models;
using Veyrin.Config.Providers;
using Veyrin.Config.Utils;

namespace Veyrin.Demo.CFG;

public class CfgDemo
{
    public static void Main(string[] args)
    {
        // 1. 初始化 ConfigHelper（使用 3種Provider）
        string basePath = ConfigPathUtils.AppBasePath;
        string env = EnvHelper.Default.EnvironmentName;
        var configFiles = new List<ConfigFileDescriptor>
        {
            new ("appsettings.json", ConfigFileType.Json),
            new ("customsettings.ini", ConfigFileType.Ini)
        };
        ConfigHelper.Initialize(
            new JsonConfigProvider(basePath, env),
            new EnvironmentVariableProvider(),
            new FileConfigLoader(configFiles)
        );

        // 2. 取得單一設定值
        string appName = ConfigHelper.Get("AppSettings:AppName", "DefaultApp");
        int maxItems = ConfigHelper.Get("AppSettings:MaxItems", 100);
        bool enableFeature = ConfigHelper.Get("Features:EnableNewFeature", false);

        Console.WriteLine($"AppName: {appName}");
        Console.WriteLine($"MaxItems: {maxItems}");
        Console.WriteLine($"EnableFeature: {enableFeature}");

        // 3. 取得 Section 並轉成物件
        var dbConfig = ConfigHelper.GetSection<DatabaseConfig>("Database");
        Console.WriteLine($"DB Host: {dbConfig.Host}, Port: {dbConfig.Port}");

        // 4. 監聽設定檔變更
        ConfigHelper.OnReload += () =>
        {
            Console.WriteLine("設定檔已重新載入！");
            // 可重新取得最新設定
            var newMaxItems = ConfigHelper.Get("AppSettings:MaxItems", 100);
            Console.WriteLine($"Updated MaxItems: {newMaxItems}");
        };

        Console.WriteLine("按任意鍵結束...");
        Console.ReadKey();
    }
}

// 假設有一個 DatabaseConfig 類別對應 appsettings.json 的 Database Section
public class DatabaseConfig
{
    public string Host { get; set; } = string.Empty;
    public int Port { get; set; }
    public string User { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}