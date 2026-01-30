using System.Diagnostics.CodeAnalysis;

namespace Veyrin.Core.Conversion;

public static class ConversionUtils
{
    public static decimal ToDecimal(this string str, decimal defaultVal = decimal.Zero) => decimal.TryParse(str, out decimal result) ? result : defaultVal;
    public static decimal ToDecimal([NotNull] this int _v) => Convert.ToDecimal(_v);
    public static int ToInt(this string? str)
    {
        if (int.TryParse(str, out int result))
            return result;
        throw new ArgumentException("Invalid int string");
    }
    public static int ToInt([NotNull] this decimal _v) => Convert.ToInt32(_v);
    public static int ToInt(this string? str, bool hasDefaultVal = false, int defaultVal = 0)
    {
        if (int.TryParse(str, out int result))
            return result;
        return hasDefaultVal ? defaultVal : int.MinValue;
    }
    public static DateTime? ParseNullableDate(this string? dateStr) => DateTime.TryParse(dateStr, out var dt) ? dt : null;
}