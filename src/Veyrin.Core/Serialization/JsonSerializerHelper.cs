using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Veyrin.Core.Validation;

namespace Veyrin.Core.Serialization;

public static class JsonSerializerHelper
{
    private static readonly JsonSerializerSettings _defaultSettings = new()
    {
        ContractResolver = new CamelCasePropertyNamesContractResolver(),
        ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
        DateFormatString = "yyyy-MM-dd HH:mm:ss",
        NullValueHandling = NullValueHandling.Ignore
    };

    /// <summary>
    /// 強制反序列化，若結果為空則拋出異常
    /// </summary>
    public static T DeserializeJson<T>(string json)
    {
        //Guard.NotEmpty(json);
        Guard.IsJson(json);
        var result = JsonConvert.DeserializeObject<T>(json, _defaultSettings);
        return result ?? throw new JsonSerializationException("Deserialized object is null");
    }

    /// <summary>
    /// 安全反序列化，失敗或為空時回傳 default
    /// </summary>
    public static T? DeserializeOrDefault<T>(string json)
    {
        if (string.IsNullOrWhiteSpace(json)) return default;
        try
        {
            return JsonConvert.DeserializeObject<T>(json, _defaultSettings);
        }
        catch
        {
            return default;
        }
    }

    /// <summary>
    /// 物件序列化為 JSON 字串
    /// </summary>
    public static string Serialize<T>(T value) => JsonConvert.SerializeObject(value, _defaultSettings);
}
    //public static void Create()
    //{
    //    System.Net.Http.Json.JsonContent.Create(new { });
    //    JsonSerializer.
    //}
