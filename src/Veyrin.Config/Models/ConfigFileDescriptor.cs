namespace Veyrin.Config.Models;

public class ConfigFileDescriptor
{
    public ConfigFileDescriptor()
    {
        FileName = "appsettings.json";
        FileType = ConfigFileType.Json;
    }
    public ConfigFileDescriptor(string fileName)
    {
        FileName = fileName;
        FileType = ConfigFileType.Json;
    }
    public ConfigFileDescriptor(string fileName, ConfigFileType fileType)
    {
        FileName = fileName;
        FileType = fileType;
    }
    public string FileName { get; set; }
    public ConfigFileType FileType { get; set; }
}
