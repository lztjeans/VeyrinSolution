using Microsoft.Extensions.Configuration;
using Veyrin.Config.Abstractions;

namespace Veyrin.Config.Providers;

public sealed class EnvironmentVariableProvider : IConfigLoader
{
    private readonly string? _prefix;

    public EnvironmentVariableProvider(string? prefix = null)
    {
        _prefix = prefix;
    }

    public IConfigurationRoot Load()
    {
        var builder = new ConfigurationBuilder();
        if (StringUtils.IsEmpty(_prefix))
            builder.AddEnvironmentVariables();
        else
            builder.AddEnvironmentVariables(_prefix);
        return builder.Build();
    }

}
