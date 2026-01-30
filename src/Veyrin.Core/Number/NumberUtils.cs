using System.Globalization;
using System.Numerics;
using System.Runtime.InteropServices;

public static class NumberUtils
{
    //************************************************************************************//
    // SubtractAndClamp
    /// <summary>
    /// 相減後回傳大於0的值<br></br>
    /// IF(I26-K26>0,I26-K26,0)
    /// </summary>
    public static int SubtractAndClamp(int x, int y) => Math.Max(x - y, 0);
    public static decimal SubtractAndClamp(decimal x, decimal y) => Math.Max(x - y, 0m);

    //************************************************************************************//
    // RoundUp 系列方法
    /// <summary>
    /// 無條件取到最接近的數字
    /// </summary>
    public static decimal RoundUpToNearest(decimal number, int step = 1) => Math.Ceiling(number / step) * step;

    /// <summary>
    /// 四捨五入到最接近的數字
    /// </summary>
    /// <param name="step">小數位數</param>
    public static decimal RoundUp(decimal number, int step = 0) => Math.Round(number, step, MidpointRounding.AwayFromZero);
    public static decimal RoundUp(double number, int step = 0) => RoundUp((decimal)number, step);

    //************************************************************************************//
    // ToNumber 通用數字轉換
    /// <summary>
    /// 通用、安全的數字轉換方法
    /// 支援 string、int、long、float、double、decimal。
    /// 永不丟例外，轉換失敗回傳 defaultVal。
    /// </summary>
    public static T ToNumber<T>(this object? value, T defaultVal = default!) where T : struct, IConvertible
    {
        if (value == null)
            return defaultVal;

        try
        {
            // 若已是目標型別，直接回傳
            if (value is T tVal)
                return tVal;

            // 處理字串輸入
            if (value is string s)
            {
                if (string.IsNullOrWhiteSpace(s))
                    return defaultVal;

                if (double.TryParse(s, NumberStyles.Any, CultureInfo.InvariantCulture, out double dbl))
                    return (T)Convert.ChangeType(dbl, typeof(T), CultureInfo.InvariantCulture);

                return defaultVal;
            }

            // 其餘數值型別嘗試轉型
            if (value is IConvertible conv)
                return (T)Convert.ChangeType(conv, typeof(T), CultureInfo.InvariantCulture);
        }
        catch
        {
            // 失敗回傳預設值
        }

        return defaultVal;
    }

    //************************************************************************************//
    // SplitNumber
    /// <summary>
    /// 將數字拆分成整數部分與小數部分
    /// </summary>
    public static (decimal integerPart, double fractionalPart) SplitNumber(this decimal number)
    {
        if (number < 0) throw new Exception("Must be a positive number");
        decimal integerPart = Math.Floor(number);
        double fractionalPart = (double)(number - integerPart);
        return (integerPart, fractionalPart);
    }

    //******************************************************************************************//

    public static bool IsAtLeast<T>(this T value, T min) where T : INumber<T> => value >= min;
    public static bool IsAtMost<T>(this T value, T min) where T : INumber<T> => value <= min;
    public static bool IsMoreThan<T>(this T value, T min) where T : INumber<T> => value > min;
    public static bool IsLessThan<T>(this T value, T min) where T : INumber<T> => value < min;
    //public static bool IsAtLeast<T>(this T value, T min) where T : INumber<T> => value >= min;
    
}
