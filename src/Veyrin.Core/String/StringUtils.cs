using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;


public static class StringUtils
{
    public static string SplitAtIndex(this string? str, string separator, int xth = 0)
    {
        if (IsEmpty(str)) return string.Empty;
        string[] sa = str!.Split(separator);
        if (xth >= sa.Length) return string.Empty;
        return sa[xth];
    }

    public static string[] Split(string? str, string separator) => str?.Split(separator) ?? [];
    public static bool Equals(this string? a, string? b)
    {
        if (a == null && b == null) return true;
        if (a == null || b == null) return false;
        return a.Equals(b);
    }
    public static bool NotEquals(string? a, string? b) => !Equals(a, b);
    public static bool EqualsIgnoreCase(string? a, string? b)
    {
        if (a == null && b == null) return true;
        if (a == null || b == null) return false;
        return a.Equals(b, StringComparison.OrdinalIgnoreCase);
    }
    public static bool NotEqualsIgnoreCase(string? a, string? b) => !EqualsIgnoreCase(a, b);
    public static bool IsEmpty([NotNullWhen(false)]this string? str) => string.IsNullOrEmpty(str);
    public static bool IsNotEmpty([NotNullWhen(true)] string? str) => !IsEmpty(str);
    public static string TrimToEmpty(string? str) => str?.Trim() ?? string.Empty;
    public static string? TrimToNull(string? str) => IsEmpty(str) ? null : str!.Trim();
    public static string ToLower(string? str) => str?.ToLower() ?? string.Empty;
    public static string ToUpper(string? str) => str?.ToUpper() ?? string.Empty;
    public static string Replace(string? str, string oldStr, string newStr, bool ignoreCase = false) => str?.Replace(oldStr, newStr, ignoreCase ? StringComparison.CurrentCultureIgnoreCase : StringComparison.CurrentCulture) ?? string.Empty;
    public static string ReplaceAll(string? str, string oldStr, string newStr, bool ignoreCase = false) => str?.Replace(oldStr, newStr, ignoreCase ? StringComparison.CurrentCultureIgnoreCase : StringComparison.CurrentCulture) ?? string.Empty;
    public static string Substring(string? str, int start) => str?[start..] ?? string.Empty;
    public static string Substring(string? str, int start, int length)
    {
        if (IsEmpty(str)) return string.Empty;
        if (start < 0 || start >= str!.Length || length <= 0) return string.Empty;
        length = Math.Min(length, str.Length - start);
        return str.Substring(start, length);
    }
    public static string Concat(string? a, string? b) => a + b;
    public static string Concat(string? a, string? b, string? c) => a + b + c;
    public static string Concat(this IEnumerable<string> strings, string? separator = "") => string.Join(separator, strings);
    public static string Concat(string? separator, params object?[] values) => string.Join(separator, values);
    public static string LeftPad(string? str, int size, char padChar) => str?.PadLeft(size, padChar) ?? string.Empty;
    public static string RightPad(string? str, int size, char padChar) => str?.PadRight(size, padChar) ?? string.Empty;
    public static bool StartsWith(string? a, string? b)
    {
        if (a == null && b == null) return true;
        if (a == null || b == null) return false;
        return a.StartsWith(b, StringComparison.OrdinalIgnoreCase);
    }
    public static bool EndsWith(string? a, string? b)
    {
        if (a == null && b == null) return true;
        if (a == null || b == null) return false;
        return a.EndsWith(b, StringComparison.OrdinalIgnoreCase);
    }
    public static bool Contains(string? a, string? b)
    {
        if (a == null && b == null) return true;
        if (a == null || b == null) return false;
        return a.Contains(b);
    }
    public static bool ContainsIgnoreCase(string? a, string? b)
    {
        if (a == null && b == null) return true;
        if (a == null || b == null) return false;
        return a.Contains(b, StringComparison.OrdinalIgnoreCase);
    }
    //public static string? GetFirst(List<string?> list) => list.Count > 0 ? list[0] : null;
    //public static string? GetLast(List<string?> list) => list.Count > 0 ? list[^1] : null;
    //public static string? Get(List<string?> list, int index) => list.Count > index ? list[index] : null;
    //public static string? Get(List<string?> list, int index, string? defaultValue) => list.Count > index ? list[index] : defaultValue;
    //public static string? GetFirst(string?[] list) => list.Length > 0 ? list[0] : null;
    //public static string? GetLast(string?[] list) => list.Length > 0 ? list[^1] : null;
    //public static string? Get(string?[] list, int index) => list.Length > index ? list[index] : null;
    //public static string? Get(string?[] list, int index, string? defaultValue) => list.Length > index ? list[index] : defaultValue;

    public static bool IsMatch(this string input, string pattern, out Match? match)
    {
        if (IsEmpty(input))
        {
            match = null;
            return false;
        }
        match = Regex.Match(input, pattern);
        return match.Success;
    }

    public static string TrimAllToEmpty(this string value)
    {
        if (string.Equals(value, "All", StringComparison.OrdinalIgnoreCase))
            return string.Empty;
        return value;
    }

    public static string Truncate(string value, int maxLength)
    {
        if (string.IsNullOrEmpty(value)) return value;
        return value.Length <= maxLength ? value : value[..maxLength] + "...(truncated)";
    }

}
