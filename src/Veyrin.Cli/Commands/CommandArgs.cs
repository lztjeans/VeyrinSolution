namespace Veyrin.Cli.Commands;

public class CommandArgs
{
    // 存放選項： key -> value (例如: --port -> 8080)
    private readonly Dictionary<string, string> _options = new(StringComparer.OrdinalIgnoreCase);
    // 存放標誌： key (例如: --verbose)
    private readonly HashSet<string> _flags = new(StringComparer.OrdinalIgnoreCase);
    // 存放剩餘的匿名參數 (例如: file1.txt, file2.txt)
    private readonly List<string> _arguments = [];

    public CommandArgs(string[] args)
    {
        Parse(args);
    }

    private void Parse(string[] args)
    {
        for (int i = 0; i < args.Length; i++)
        {
            string arg = args[i];

            if (arg.StartsWith('-'))
            {
                // 處理 --key=value 格式
                if (arg.Contains('='))
                {
                    var parts = arg.Split('=', 2);
                    _options[parts[0].TrimStart('-')] = parts[1];
                }
                // 處理 -key value 格式
                else if (i + 1 < args.Length && !args[i + 1].StartsWith('-'))
                {
                    _options[arg.TrimStart('-')] = args[++i];
                }
                // 處理 Flag (無值)
                else
                {
                    _flags.Add(arg.TrimStart('-'));
                }
            }
            else
            {
                _arguments.Add(arg);
            }
        }
    }

    #region 讀取方法

    public string? GetOption(string key) => _options.TryGetValue(key, out var val) ? val : null;

    public bool HasFlag(string key) => _flags.Contains(key);

    public string? GetArgument(int index) => _arguments.Count > index ? _arguments[index] : null;

    public T? GetOption<T>(string key, T? defaultValue = default)
    {
        var val = GetOption(key);
        if (val == null) return defaultValue;

        try { return (T)Convert.ChangeType(val, typeof(T)); }
        catch { return defaultValue; }
    }

    #endregion
}
