using Microsoft.Extensions.Configuration;

namespace Veyrin.Config.Abstractions;

public interface IConfigLoader
{
    IConfigurationRoot Load();
}
