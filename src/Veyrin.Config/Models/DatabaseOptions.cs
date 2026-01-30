namespace Veyrin.Config.Models;

public class DatabaseOptions
{
    public string ConnectionString { get; set; } = string.Empty;
    public int TimeoutSeconds { get; set; } = 30;
}
