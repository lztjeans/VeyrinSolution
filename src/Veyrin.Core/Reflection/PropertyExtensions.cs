using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Veyrin.Core.Reflection;

public static class PropertyExtensions
{
    /// <summary>
    /// 取得物件所有公開屬性的值（從快取讀取）
    /// </summary>
    public static IDictionary<string, object?> GetPropertyValues(this object instance)
    {
        if (instance == null) return new Dictionary<string, object?>();

        var type = instance.GetType();
        var props = ReflectionCache.GetProperties(type);

        return props.ToDictionary(
            p => p.Name,
            p => p.GetValue(instance)
        );
    }

    /// <summary>
    /// 檢查屬性是否帶有特定的 Attribute
    /// </summary>
    public static bool HasAttribute<T>(this PropertyInfo prop) where T : Attribute
    {
        return ReflectionCache.GetAttributes<T>(prop).Length > 0;
    }
}
