using Microsoft.Extensions.Configuration;
using Veyrin.Config.Abstractions;

namespace Veyrin.Config.Providers;

public sealed class JsonConfigProvider : IConfigLoader
{
    private readonly string _basePath;
    private readonly string _env;

    public JsonConfigProvider(string basePath, string env)
    {
        _basePath = basePath;
        _env = env;
    }

    public IConfigurationRoot Load()
    {
        return new ConfigurationBuilder()
            .SetBasePath(_basePath)
            .AddJsonFile("appsettings.json", optional: false)
            .AddJsonFile($"appsettings.{_env}.json", optional: true)
            .AddEnvironmentVariables()
            .Build();
    }

}
