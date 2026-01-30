using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Veyrin.Core.Validation;

namespace Veyrin.Testing.Fakes;

public static class FakeFactory
{
    // 快速產生一個不輸出的 Logger，讓測試專注於邏輯而非 Log 噴發
    public static ILogger<T> CreateNullLogger<T>() => NullLogger<T>.Instance;

    // 快速模擬配置環境
    public static IConfiguration CreateFakeConfig(Dictionary<string, string> settings)
    {
        Guard.NotEmpty(settings);

        return new ConfigurationBuilder()
            .AddInMemoryCollection(settings!)
            .Build();
    }
}