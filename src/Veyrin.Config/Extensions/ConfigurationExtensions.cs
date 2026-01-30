using Microsoft.Extensions.Configuration;

namespace Veyrin.Config.Extensions;

public static class ConfigurationExtensions
{
    public static T BindSection<T>(this IConfiguration config, string section)
        where T : new()
    {
        var obj = new T();
        config.GetSection(section).Bind(obj);
        return obj;
    }
}
