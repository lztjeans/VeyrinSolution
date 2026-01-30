using System.Diagnostics.CodeAnalysis;
using System.Numerics;
using System.Runtime.CompilerServices;
using Veyrin.Core.Exceptions;
using Veyrin.Core.Models;

namespace Veyrin.Core.Validation;

public static partial class Guard
{
    /// <summary>
    /// 基礎布林斷言：如果 condition 為 false (條件不成立)，則拋出驗證異常
    /// </summary>
    /// <param name="condition">要測試的布林條件</param>
    /// <param name="message">驗證失敗時的錯誤訊息</param>
    /// <exception cref="ValidationException">當條件為 false 時拋出</exception>
    public static void IsFalse(bool condition, string message)
    {
        if (!condition)
            throw new ValidationException(message);
    }

    /// <summary>
    /// 確保條件必須為 true：如果 condition 為 false，則拋出驗證異常
    /// </summary>
    /// <remarks>
    /// 此方法通常用於檢查系統狀態或初始化標記 (例如：Guard.IsTrue(_isInitialized, "尚未初始化"))
    /// </remarks>
    /// <param name="condition">必須為 true 的布林值</param>
    /// <param name="message">當條件為 false (未通過驗證) 時的錯誤訊息</param>
    public static void IsTrue(bool condition, string message) => IsFalse(condition, message);
    public static void FileExists(string path)
    {
        if (!File.Exists(path))
            throw new ValidationException($"找不到指定檔案: {path}");
    }


    /// <summary>
    /// 物件不能為 Null
    /// </summary>
    public static void NotNull(object? value, [CallerArgumentExpression("value")] string paramName = "", string message = "")
    {
        if (value == null)
        {
            message = message.IsEmpty() ? $"{paramName} cannot be null." : message;
            throw new ValidationException(message, paramName);
        }
    }

    /// <summary>
    /// 整合您現有的字串工具類別
    /// </summary>
    public static void NotEmpty([NotNullWhen(true)] string? value, [CallerArgumentExpression("value")] string paramName = "", string message = "")
    {
        // 這裡調用你現有的工具類別
        if (StringUtils.IsEmpty(value))
        {
            message = message.IsEmpty() ? $"{paramName} cannot be null or empty." : message;
            throw new ValidationException(message);
        }
    }

    /// <summary>
    /// 確保數值在指定值「以上」(>= min)。
    /// </summary>
    public static void AtLeast<T>(this T value, T min, [CallerArgumentExpression("value")] string paramName = "")
        where T : INumber<T>
    {
        // 呼叫您的泛型工具類別 (假設已實作泛型版 IsAtLeast)
        if (!value.IsAtLeast(min))
            throw new ValidationException($"{paramName} must be at least {min}.", paramName);
    }

    /// <summary>
    /// 確保數值在指定值「以下」(<= max)。
    /// </summary>
    public static void AtMost<T>(this T value, T max, [CallerArgumentExpression("value")] string paramName = "")
        where T : INumber<T>
    {
        if (!value.IsAtMost(max))
            throw new ValidationException($"{paramName} must be at most {max}.", paramName);
    }

    /// <summary>
    /// 確保數值「超過」指定值(> min)。
    /// </summary>
    public static void MoreThan<T>(this T value, T min, [CallerArgumentExpression("value")] string paramName = "")
        where T : INumber<T>
    {
        if (!value.IsMoreThan(min))
            throw new ValidationException($"{paramName} must be more than {min}.", paramName);
    }

    /// <summary>
    /// 確保數值「未滿」指定值(< max)。
    /// </summary>
    public static void LessThan<T>(this T value, T max, [CallerArgumentExpression("value")] string paramName = "")
        where T : INumber<T>
    {
        if (!value.IsLessThan(max))
            throw new ValidationException($"{paramName} must be less than {max}.", paramName);
    }

    /// <summary>
    /// 範圍檢查，根據指定的邊界行為驗證數值是否在區間內
    /// </summary>
    /// <typeparam name="T">必須為數值型別</typeparam>
    /// <param name="value">待檢查數值</param>
    /// <param name="min">最小值</param>
    /// <param name="max">最大值</param>
    /// <param name="paramName">參數名稱</param>
    /// <param name="boundary">邊界行為，預設為全包含 (Inclusive)</param>
    public static void InRange<T>(
        this T value,
        T min,
        T max,
        [CallerArgumentExpression("value")] string paramName = "",
        RangeBoundary boundary = RangeBoundary.Inclusive) where T : INumber<T>
    {
        // 防禦性檢查：min 不應大於 max
        if (min > max)
            throw new ArgumentException("Min value cannot be greater than max value.", nameof(min));

        bool isValid = boundary switch
        {
            RangeBoundary.Inclusive => value.IsAtLeast(min) && value.IsAtMost(max),
            RangeBoundary.Exclusive => value.IsMoreThan(min) && value.IsLessThan(max),
            RangeBoundary.InclusiveMin => value.IsAtLeast(min) && value.IsLessThan(max),
            RangeBoundary.InclusiveMax => value.IsMoreThan(min) && value.IsAtMost(max),
            _ => throw new ArgumentOutOfRangeException(nameof(boundary))
        };

        if (!isValid)
        {
            string message = GetRangeErrorMessage(min, max, boundary);
            throw new ValidationException($"{paramName} {message}", paramName);
        }
    }

    private static string GetRangeErrorMessage<T>(T min, T max, RangeBoundary boundary)
    {
        return boundary switch
        {
            RangeBoundary.Inclusive => $"must be between {min} and {max} (inclusive).",
            RangeBoundary.Exclusive => $"must be between {min} and {max} (exclusive).",
            RangeBoundary.InclusiveMin => $"must be at least {min} and less than {max}.",
            RangeBoundary.InclusiveMax => $"must be more than {min} and at most {max}.",
            _ => "is out of range."
        };
    }


}
