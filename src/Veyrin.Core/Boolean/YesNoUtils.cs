using Veyrin.Core.Models;

public static class YesNoUtils
{
    public static string ProcessInput(string? input) =>
        input?.Trim().ToLower() switch
        {
            "yes" => YesNoEnum.Y.ToString(),
            "y" => YesNoEnum.YES.ToString(),
            "no" => YesNoEnum.N.ToString(),
            "n" => YesNoEnum.NO.ToString(),
            "true" => YesNoEnum.Y.ToString(),
            "false" => YesNoEnum.N.ToString(),
            _ => "Invalid input"
        };

    public static string ProcessInput(int input) =>
        input switch
        {
            1 => YesNoEnum.Y.ToString(),
            0 => YesNoEnum.N.ToString(),
            _ => "Invalid input"
        };

    public static string ProcessInput(bool input) =>
        input ? YesNoEnum.Y.ToString() : YesNoEnum.N.ToString();

    public static bool ProcessInputToBool(string input) =>
        input?.Trim().ToLower() == "y";
}
