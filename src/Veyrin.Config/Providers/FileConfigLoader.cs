using Microsoft.Extensions.Configuration;
using Veyrin.Config.Abstractions;
using Veyrin.Config.Environments;
using Veyrin.Config.Models;
using Veyrin.Config.Utils;

namespace Veyrin.Config.Providers;

public sealed class FileConfigLoader : IConfigLoader
{
    private readonly IEnumerable<ConfigFileDescriptor> _files;

    public FileConfigLoader(IEnumerable<ConfigFileDescriptor> files)
    {
        _files = files;
    }
    public IConfigurationRoot Load()
    {
        string basePath = ConfigPathUtils.CurrentDirectory;

        var builder = new ConfigurationBuilder().SetBasePath(basePath);

        foreach (var file in _files)
        {
            AddFile(builder, file.FileName, file.FileType, false);
            AddFile(builder, InsertEnv(file.FileName, EnvHelper.Default.EnvironmentName), file.FileType, true);
        }

        return builder.Build();
    }

    private static void AddFile(IConfigurationBuilder builder, string fileName, ConfigFileType type, bool optional)
    {
        switch (type)
        {
            case ConfigFileType.Json:
                builder.AddJsonFile(fileName, optional, reloadOnChange: true);
                break;
            case ConfigFileType.Ini:
                builder.AddIniFile(fileName, optional, reloadOnChange: true);
                break;
            case ConfigFileType.Xml:
                builder.AddXmlFile(fileName, optional, reloadOnChange: true);
                break;
        }
    }

    private static string InsertEnv(string name, string env)
    {
        var ext = Path.GetExtension(name);
        var baseName = Path.GetFileNameWithoutExtension(name);
        return $"{baseName}.{env}{ext}";
    }

}
