using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Primitives;

namespace Veyrin.Config.Services;

public sealed class ConfigReloadService
{
    public event Action? OnReload;

    public ConfigReloadService(IConfigurationRoot root)
    {
        ChangeToken.OnChange(
            root.GetReloadToken,
            () => OnReload?.Invoke());
    }
}
