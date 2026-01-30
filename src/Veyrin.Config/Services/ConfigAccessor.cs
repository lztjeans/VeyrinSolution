using Microsoft.Extensions.Configuration;
using Veyrin.Config.Abstractions;

namespace Veyrin.Config.Services;

public sealed class ConfigAccessor : IConfigAccessor
{
    private readonly IConfiguration _config;

    public ConfigAccessor(IConfiguration config)
    {
        _config = config;
    }

    public T Get<T>(string key, T defaultValue = default!)
    {
        var value = _config[key];
        if (string.IsNullOrEmpty(value))
            return defaultValue;

        try
        {
            if (typeof(T).IsEnum)
                return (T)Enum.Parse(typeof(T), value, true);

            return (T)Convert.ChangeType(value, typeof(T));
        }
        catch
        {
            return defaultValue;
        }
    }

    public T GetSection<T>(string section) where T : new()
        => _config.GetSection(section).Get<T>() ?? new T();
}
