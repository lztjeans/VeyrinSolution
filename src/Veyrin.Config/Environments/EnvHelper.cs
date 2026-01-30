using Veyrin.Core.Models;

namespace Veyrin.Config.Environments;

public sealed class EnvHelper
{


    // 1. 實作單例：建立一個全域預設執行個體
    // 使用 static readonly 確保執行緒安全且只會被初始化一次
    public static EnvHelper Default { get; } = new EnvHelper();

    private readonly string _name;

    // 2. 修改建構函式：讓參數可選 (預設為 null)
    public EnvHelper(string? name = null)
    {
        if (StringUtils.IsNotEmpty(name))
        {
            _name = name;
        }
        else
        {
            // 優先權：傳入參數 > ASPNETCORE_ENVIRONMENT > DOTNET_ENVIRONMENT > Production
            _name = Environment.GetEnvironmentVariable(EnvironmentNames.AspnetCoreEnv) ??
                    Environment.GetEnvironmentVariable(EnvironmentNames.DotnetCoreEnv) ??
                    EnvironmentNames.Production;
        }
    }

    // 3. 實例方法：判斷該執行個體的環境
    public bool IsDevelopment() => StringUtils.EqualsIgnoreCase(EnvironmentNames.Development, _name);
    public bool IsStaging() => StringUtils.EqualsIgnoreCase(EnvironmentNames.Staging, _name);
    public bool IsProduction() => StringUtils.EqualsIgnoreCase(EnvironmentNames.Production, _name);

    public string EnvironmentName => _name;
    public static bool IsTesting() =>
    AppDomain.CurrentDomain.GetAssemblies()
        .Any(a => a.FullName!.Contains("test", StringComparison.OrdinalIgnoreCase));
}
