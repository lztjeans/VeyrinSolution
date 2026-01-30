using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Veyrin.Core.Reflection;

public static class TypeUtils
{
    /// <summary>
    /// 建立類型實例（支援無參數建構子）
    /// </summary>
    public static T? CreateInstance<T>() where T : class, new() => new T();

    /// <summary>
    /// 動態建立實例（透過 Type）
    /// </summary>
    public static object? CreateInstance(Type type)
    {
        try
        {
            return Activator.CreateInstance(type);
        }
        catch
        {
            return null;
        }
    }

    /// <summary>
    /// 判斷類型是否為可空型別 (Nullable<T>)
    /// </summary>
    public static bool IsNullable(Type type) =>
        type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>);
}
