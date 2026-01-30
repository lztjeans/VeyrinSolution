using Veyrin.Core.Reflection;

namespace Veyrin.Data.Converters;

public static class ValueConverter
{
    /// <summary>
    /// 安全的轉型，處理 DBNull 與預設值
    /// </summary>
    public static T? ToValue<T>(object? value)
    {
        if (value == null || value == DBNull.Value) return default;

        try
        {
            var targetType = typeof(T);

            // 處理 Nullable 型別
            if (TypeUtils.IsNullable(targetType))
            {
                targetType = Nullable.GetUnderlyingType(targetType)!;
            }

            // 處理 Enum
            if (targetType.IsEnum)
            {
                return (T)Enum.Parse(targetType, value.ToString()!);
            }

            return (T)Convert.ChangeType(value, targetType);
        }
        catch
        {
            return default;
        }
    }
}
