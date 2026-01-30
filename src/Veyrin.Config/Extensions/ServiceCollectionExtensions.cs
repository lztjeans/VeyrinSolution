using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Veyrin.Config.Abstractions;
using Veyrin.Config.Models;
using Veyrin.Config.Providers;
using Veyrin.Config.Services;

namespace Veyrin.Config.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddConfig(
        this IServiceCollection services,
        params ConfigFileDescriptor[] files)
    {
        var loader = new FileConfigLoader(files);
        var configRoot = loader.Load();

        services.AddSingleton<IConfiguration>(configRoot);
        services.AddSingleton<IConfigAccessor, ConfigAccessor>();
        services.AddSingleton(new ConfigReloadService(configRoot));

        return services;
    }
}
