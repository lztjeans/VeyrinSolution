using Microsoft.AspNetCore.Http;
using System.Collections;
using System.Globalization;
using System.Reflection;
using System.Text.RegularExpressions;

namespace Veyrin.Vista;

public static class FormMappingExtensions
{
    private static readonly Regex SegmentRegex = new(@"(\[[^\]]+\])", RegexOptions.Compiled);

    /// <summary>
    /// 根據傳入的單一物件類型 T，從 IFormCollection 映射相關的鍵值對。
    /// T 可以是頂層物件 (如 FormData)，也可以是子物件 (如 LeafFrame 或 DieRow)。
    /// </summary>
    public static T? MapFormToObject<T>(this IFormCollection form) where T : new()
    {
        T targetObject = new();
        Type targetType = typeof(T);

        // 嘗試根據目標類別名稱，推斷其可能的前綴（例如 LeafFrame -> "lf_"）
        string expectedPrefix = targetType.Name.ToLowerInvariant() + "_";

        foreach (var key in form.Keys)
        {
            string? formValue = form[key].ToString();
            if (string.IsNullOrEmpty(formValue)) continue;

            // 1. 處理簡單/底線分隔的鍵 (例如：r_ddl_rlvl, lf_LFSize)
            if (!key.Contains('['))
            {
                // 檢查此鍵是否與目標 T 相關 (如果 T 是 LeafFrame，只處理 lf_ 開頭的鍵)
                if (key.StartsWith(expectedPrefix, StringComparison.OrdinalIgnoreCase))
                {
                    // 移除 T 的特定前綴 (例如移除 lf_)
                    string rawChildName = key[expectedPrefix.Length..];

                    // 淨化子屬性名稱 (例如: "ddl_lftype" -> "Lftype")
                    string cleanedChildName = CleanSimplePropertyName(rawChildName);

                    // 在 T 物件上找到並設定屬性
                    PropertyInfo? property = targetType.GetProperty(cleanedChildName, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
                    if (property != null)
                    {
                        SetSimplePropertyValue(targetObject, property, formValue);
                    }
                }
                // 處理不帶前綴的簡單屬性 (例如：如果 T 是 FormData，並且表單中有一個不帶前綴的欄位)
                else
                {
                    // 淨化屬性名稱 (例如: "r_ddl_rlvl" -> "Rlvl")
                    string cleanedPropertyName = CleanSimplePropertyName(key);

                    PropertyInfo? property = targetType.GetProperty(cleanedPropertyName, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
                    if (property != null)
                    {
                        SetSimplePropertyValue(targetObject, property, formValue);
                    }
                }
            }
            // 2. 處理列表格式的鍵 (例如：die_rows[0][chip])
            else
            {
                int firstBracketIndex = key.IndexOf('[');
                if (firstBracketIndex <= 0) continue;

                // 頂層列表屬性名稱 (例如: "die_rows")
                string collectionName = key.Substring(0, firstBracketIndex);

                // 檢查 T 是否擁有這個列表屬性 (例如 T=FormData，它有 DieRows 屬性)
                PropertyInfo? collectionProperty = targetType.GetProperty(collectionName, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);

                if (collectionProperty != null)
                {
                    string path = key.Substring(firstBracketIndex);

                    // 提取路徑片段，例如 "[0]", "[chip]" -> "0", "chip"
                    var matches = SegmentRegex.Matches(path).Cast<Match>().Select(m => m.Groups[1].Value.Trim('[', ']')).ToArray();

                    if (matches.Length > 0 && IsList(collectionProperty.PropertyType))
                    {
                        // 遞迴處理列表結構
                        ProcessListPath(targetObject, collectionProperty, matches, formValue);
                    }
                }
            }
        }

        return targetObject;
    }

    /// <summary>
    /// 淨化簡單屬性名稱：移除 ddl_ 前綴，並處理 r_, lf_ 等頂層前綴，然後將第一個字母大寫 (遵循 C# 命名慣例)。
    /// </summary>
    private static string CleanSimplePropertyName(string rawName)
    {
        string cleanedName = rawName;

        // 移除 ddl_
        if (cleanedName.StartsWith("ddl_", StringComparison.OrdinalIgnoreCase))
        {
            cleanedName = cleanedName.Substring("ddl_".Length);
        }

        // 移除 r_, lf_ 頂層前綴 (此時應只剩下 r_ 或 lf_)
        if (cleanedName.StartsWith("r_", StringComparison.OrdinalIgnoreCase))
        {
            cleanedName = cleanedName.Substring("r_".Length);
        }
        else if (cleanedName.StartsWith("lf_", StringComparison.OrdinalIgnoreCase))
        {
            // 在此方法中，如果 T 不是 LeafFrame，則 lf_ 在 SetSimpleOrNestedProperty 外不應被移除
            // 這裡保留，以防傳入 T 是像 FormData 這樣的頂層物件
            cleanedName = cleanedName.Substring("lf_".Length);
        }

        // 確保結果是 PascalCase，例如 rlvl -> Rlvl
        if (!string.IsNullOrEmpty(cleanedName) && char.IsLower(cleanedName[0]))
        {
            return char.ToUpper(cleanedName[0]) + cleanedName.Substring(1);
        }

        return cleanedName;
    }

    /// <summary>
    /// 處理列表 (List<T>) 結構的遞迴邏輯。
    /// </summary>
    private static void ProcessListPath(object currentObject, PropertyInfo collectionProperty, string[] remainingSegments, string value)
    {
        Type? itemType = GetListItemType(collectionProperty.PropertyType);
        if (itemType == null) return;

        object? propertyValue = collectionProperty.GetValue(currentObject);

        // 確保列表已實例化
        if (propertyValue == null)
        {
            propertyValue = Activator.CreateInstance(collectionProperty.PropertyType);
            collectionProperty.SetValue(currentObject, propertyValue);
        }

        var list = (IList)propertyValue;
        string nextSegment = remainingSegments[0];

        if (int.TryParse(nextSegment, out int index))
        {
            // 確保列表大小足夠
            while (list.Count <= index)
            {
                list.Add(Activator.CreateInstance(itemType));
            }

            // 列表中的項目
            object listItem = list[index]!;

            // 列表中的項目屬性名稱在 remainingSegments[1]
            if (remainingSegments.Length > 1)
            {
                string nextPropertyName = remainingSegments[1];
                PropertyInfo? nextProperty = itemType.GetProperty(nextPropertyName, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
                if (nextProperty != null)
                {
                    // 執行最終賦值
                    SetSimplePropertyValue(listItem, nextProperty, value);
                }
            }
        }
    }


    /// <summary>
    /// 將字串值轉換並賦值給簡單屬性。
    /// </summary>
    private static void SetSimplePropertyValue(object targetObject, PropertyInfo property, string formValue)
    {
        try
        {
            // [保持與前一個版本一致的類型轉換邏輯]
            Type propertyType = property.PropertyType;
            bool isNullable = propertyType.IsGenericType && propertyType.GetGenericTypeDefinition() == typeof(Nullable<>);

            if (isNullable)
            {
                propertyType = Nullable.GetUnderlyingType(propertyType)!;
            }

            object? convertedValue = propertyType == typeof(string)
                                    ? formValue
                                    : Convert.ChangeType(
                                        formValue,
                                        propertyType,
                                        CultureInfo.InvariantCulture
                                    );

            property.SetValue(targetObject, convertedValue);
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error setting property '{property.Name}' to value '{formValue}' for object '{targetObject.GetType().Name}': {ex.Message}");
        }
    }

    // 輔助方法：檢查類型是否為 List<T>
    private static bool IsList(Type type)
    {
        return type.IsGenericType && type.GetGenericTypeDefinition() == typeof(List<>);
    }

    // 輔助方法：獲取 List<T> 中的 T 類型
    private static Type? GetListItemType(Type listType)
    {
        return listType.GetGenericArguments().FirstOrDefault();
    }
}