using System.Data;
using Veyrin.Core.Reflection;

namespace Veyrin.Data.Converters;

public static class CollectionToTableConverter
{
    public static DataTable ToDataTable<T>(this IEnumerable<T> items) where T : class
    {
        var table = new DataTable(typeof(T).Name);
        var props = ReflectionCache.GetProperties(typeof(T));

        foreach (var prop in props)
        {
            // 處理 Nullable 型別的 DataTable 欄位定義
            var propType = prop.PropertyType;
            if (propType.IsGenericType && propType.GetGenericTypeDefinition() == typeof(Nullable<>))
            {
                propType = Nullable.GetUnderlyingType(propType)!;
            }
            table.Columns.Add(prop.Name, propType);
        }

        foreach (var item in items)
        {
            var values = new object?[props.Length];
            for (int i = 0; i < props.Length; i++)
            {
                values[i] = props[i].GetValue(item) ?? DBNull.Value;
            }
            table.Rows.Add(values);
        }

        return table;
    }
}
