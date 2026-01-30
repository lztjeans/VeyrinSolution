using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Primitives;
using Veyrin.Config.Abstractions;
using Veyrin.Core.Validation;

namespace Veyrin.Config;

public static class ConfigHelper
{
    private static IConfigurationRoot? _configuration;
    private static bool _isInitialized = false;
    public static event Action? OnReload;

    public static void Initialize(params IConfigLoader[] loaders)
    {
        if (_isInitialized) return;

        var builder = new ConfigurationBuilder();
        foreach (var loader in loaders)
        {
            var config = loader.Load();
            builder.AddConfiguration(config);
        }
        _configuration = builder.Build();
        ChangeToken.OnChange(() => _configuration.GetReloadToken(), () => { OnReload?.Invoke(); });
        _isInitialized = true;
    }

    public static IConfiguration GetConfiguration()
    {
        Guard.IsTrue(_isInitialized, "ConfigHelper 尚未初始化");
        return _configuration!;
    }

    public static T Get<T>(string key, T defaultValue = default!)
    {
        Guard.IsTrue(_isInitialized, "ConfigHelper 尚未初始化");
        var value = _configuration![key];
        if (StringUtils.IsEmpty(value)) return defaultValue;
        try
        {
            return (T)Convert.ChangeType(value, typeof(T));
        }
        catch
        {
            return defaultValue;
        }
    }

    public static T GetSection<T>(string sectionName) where T : new()
    {
        Guard.IsTrue(_isInitialized, "ConfigHelper 尚未初始化");
        var section = _configuration!.GetSection(sectionName);
        if (!section.Exists()) return new T();
        return section.Get<T>() ?? new T();
    }
}