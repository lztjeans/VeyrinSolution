namespace Veyrin.Cli.Terminal;

public static class ConsoleWriter
{
    public static void Write(string message, string colorCode)
    {
        Console.Write($"{colorCode}{message}{AnsiCodes.Reset}");
    }

    public static void WriteLine(string message, string colorCode)
    {
        Console.WriteLine($"{colorCode}{message}{AnsiCodes.Reset}");
    }

    // 語義化捷徑
    public static void Success(string msg) => WriteLine($"[✔] {msg}", AnsiCodes.Green);
    public static void Error(string msg) => WriteLine($"[✘] {msg}", AnsiCodes.Red);
    public static void Warning(string msg) => WriteLine($"[!] {msg}", AnsiCodes.Yellow);
    public static void Info(string msg) => WriteLine($"[i] {msg}", AnsiCodes.Cyan);
    public static void Muted(string msg) => WriteLine(msg, AnsiCodes.Grey);

    /// <summary>
    /// 標題風格輸出
    /// </summary>
    public static void Header(string title)
    {
        Console.WriteLine();
        WriteLine($"{AnsiCodes.Bold}{title.ToUpper()}", AnsiCodes.Blue);
        WriteLine(new string('-', title.Length), AnsiCodes.Blue);
    }
}
