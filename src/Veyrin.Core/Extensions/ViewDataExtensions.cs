using Microsoft.AspNetCore.Mvc.ViewFeatures;


public static class ViewDataExtensions
{
    /// <summary>
    /// 安全取得 ViewData 的值，若不存在或型別錯誤，回傳預設值
    /// </summary>
    public static T GetValue<T>(this ViewDataDictionary viewData, string key, T defaultValue = default!)
    {
        if (viewData.TryGetValue(key, out var obj) && obj is T value)
            return value;
        return defaultValue;
    }
}
