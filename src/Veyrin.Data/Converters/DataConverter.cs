using System.Data;
using Veyrin.Core.Reflection;
using Veyrin.Core.Validation;

namespace Veyrin.Data.Converters;

public static class DataConverter
{
    /// <summary>
    /// 將 DataTable 轉換為物件列表
    /// </summary>
    public static List<T> ToList<T>(this DataTable table) where T : class, new()
    {
        if (table == null || table.Rows.Count == 0) return [];

        var list = new List<T>();
        // 利用我們在 Reflection 模組建立的快取
        var properties = ReflectionCache.GetProperties(typeof(T));

        foreach (DataRow row in table.Rows)
        {
            var item = new T();
            foreach (var prop in properties)
            {
                if (table.Columns.Contains(prop.Name) && row[prop.Name] != DBNull.Value)
                {
                    prop.SetValue(item, row[prop.Name]);
                }
            }
            list.Add(item);
        }
        return list;
    }

    /// <summary>
    /// 將單一 DataRow 轉換為物件
    /// </summary>
    public static T ToEntity<T>(this DataRow row) where T : class, new()
    {
        Guard.NotNull(row, nameof(row));

        var item = new T();
        var properties = ReflectionCache.GetProperties(typeof(T));

        foreach (var prop in properties)
        {
            if (row.Table.Columns.Contains(prop.Name) && row[prop.Name] != DBNull.Value)
            {
                prop.SetValue(item, row[prop.Name]);
            }
        }
        return item;
    }
}