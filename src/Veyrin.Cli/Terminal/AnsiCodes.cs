namespace Veyrin.Cli.Terminal;

public static class AnsiCodes
{
    public const string Reset = "\x1b[0m";
    public const string Bold = "\x1b[1m";

    // 前景色
    public const string Red = "\x1b[31m";
    public const string Green = "\x1b[32m";
    public const string Yellow = "\x1b[33m";
    public const string Blue = "\x1b[34m";
    public const string Cyan = "\x1b[36m";
    public const string Grey = "\x1b[90m";

    // 背景色
    public const string BgRed = "\x1b[41m";
}
