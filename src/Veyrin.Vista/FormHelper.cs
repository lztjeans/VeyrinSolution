using Microsoft.AspNetCore.Http;
using System.ComponentModel;
using System.Text.RegularExpressions;

namespace Veyrin.Vista;

public static class FormHelper
{
    /// <summary>
    /// 擷取欄位值：rows[{idx}][{propName}]
    /// </summary>
    public static string FetchProp(this IFormCollection form, int idx, string propName, string grpKey = "")
    {
        if (form == null) return string.Empty;

        string key = $"{grpKey}_rows[{idx}][{propName}]";
        return form.GetFormValue(key);
    }

    /// <summary>
    /// 從表單映射成單一物件
    /// </summary>
    public static T? MapFormToObject<T>(this IFormCollection form, Dictionary<string, string> keyToPropertyMap) where T : class, new()
    {
        if (form == null || keyToPropertyMap == null || keyToPropertyMap.Count == 0)
            return null;

        bool hasAnyValue = keyToPropertyMap.Keys.Any(key => !string.IsNullOrEmpty(form[key]));
        if (!hasAnyValue) return null;

        var obj = new T();
        foreach (var (formKey, propName) in keyToPropertyMap)
        {
            var value = form.GetFormValue(formKey);
            SetPropertyValue(obj, propName, value);
        }

        return obj;
    }

    /// <summary>
    /// 從表單轉成 List<T>
    /// </summary>
    public static List<T>? TransformList<T>(this IFormCollection form, string prefix, Dictionary<string, string> map) where T : new()
    {
        if (form == null || map == null || map.Count == 0)
            return null;

        var indices = FindAllIndex(form, $@"{prefix}_rows\[(\d+)\]\[");
        if (indices.Count == 0)
            return null;

        var result = new List<T>();

        foreach (var index in indices)
        {
            var obj = new T();
            foreach (var (formKey, propName) in map)
            {
                string key = $"{prefix}_rows[{index}][{formKey}]";
                string value = form.GetFormValue(key);
                SetPropertyValue(obj, propName, value);
            }

            result.Add(obj);
        }

        return result;
    }

    /// <summary>
    /// 從表單中找出所有 row index
    /// </summary>
    private static List<int> FindAllIndex(IFormCollection form, string pattern)
    {
        var regex = new Regex(pattern, RegexOptions.Compiled);
        return [.. form.Keys
        .Select(k => regex.Match(k))
        .Where(m => m.Success && m.Groups.Count > 1)
        .Select(m => int.Parse(m.Groups[1].Value))
        .Distinct()
        .OrderBy(i => i)];
    }

    /// <summary>
    /// 設定屬性值，並自動處理型別轉換
    /// </summary>
    private static void SetPropertyValue(object obj, string propName, string value)
    {
        var prop = obj.GetType().GetProperty(propName);
        if (prop != null && prop.CanWrite)
        {
            try
            {
                object? converted = ConvertToType(value, prop.PropertyType);
                prop.SetValue(obj, converted);
            }
            catch
            {
                // Optional: log or ignore
            }
        }
    }

    /// <summary>
    /// 將字串轉型為指定型別（支援 Nullable）
    /// </summary>
    private static object? ConvertToType(string value, Type targetType)
    {
        if (targetType == typeof(string)) return value;

        var underlyingType = Nullable.GetUnderlyingType(targetType) ?? targetType;

        return string.IsNullOrWhiteSpace(value)
            ? null
            : TypeDescriptor.GetConverter(underlyingType).ConvertFromInvariantString(value);
    }

    /// <summary>
    /// 嘗試從表單取得值（string）
    /// </summary>
    private static string GetFormValue(this IFormCollection form, string key)
    {
        return form.TryGetValue(key, out var value) ? value.ToString() ?? string.Empty : string.Empty;
    }

    /// <summary>
    /// 從 IFormFileCollection 擷取檔案流
    /// </summary>
    public static Stream? FetchFileFromForm(this IFormFileCollection files, string fieldName)// int? index, string fieldName = "fcnt")
    {
        if (files == null) return null;

        // 例：rows[0][fcnt]、some_prefix_rows[0][fcnt]、rows[0][other_field]
        //var pattern = $@"\[{index}\]\[{fieldName}\]$";
        //var regex = new Regex(pattern, RegexOptions.Compiled | RegexOptions.IgnoreCase);

        foreach (var file in files)
            if (StringUtils.EqualsIgnoreCase(file.Name, fieldName))
                return file.OpenReadStream();

        return null;
    }

    public static Stream? FetchFileFromFormLike(this IFormFileCollection files, string partialFieldName)
    {
        if (files == null || string.IsNullOrWhiteSpace(partialFieldName))
            return null;

        foreach (var file in files)
        {
            if (file.Name != null && file.Name.Contains(partialFieldName, StringComparison.OrdinalIgnoreCase))
            {
                return file.OpenReadStream();
            }
        }

        return null;
    }
    /// <summary>
    /// var pattern = @"rows\[0\]\[fcnt\]";
    /// Stream? fileStream = form.Files.FetchFileFromFormRegex(pattern);

    /// </summary>
    /// <param name="files"></param>
    /// <param name="pattern"></param>
    /// <returns></returns>
    public static Stream? FetchFileFromFormRegex(this IFormFileCollection files, string pattern)
    {
        if (files == null || string.IsNullOrWhiteSpace(pattern))
            return null;

        var regex = new Regex(pattern, RegexOptions.IgnoreCase | RegexOptions.Compiled);

        foreach (var file in files)
        {
            if (file.Name != null && regex.IsMatch(file.Name))
            {
                return file.OpenReadStream();
            }
        }

        return null;
    }



    public static byte[] FetchFileFromForm(this IFormFile file)
    {
        using MemoryStream stream = new();
        file.CopyTo(stream);
        return stream.ToArray();
    }
}