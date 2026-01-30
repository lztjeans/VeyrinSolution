namespace Veyrin.Config.Utils;

public static class ConfigPathUtils
{
    public static string AppBasePath => AppContext.BaseDirectory;
    public static string CurrentDirectory => Directory.GetCurrentDirectory();
}
