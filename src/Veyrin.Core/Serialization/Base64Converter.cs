using Newtonsoft.Json;
using System.IO.Compression;
using System.Text;


public static class Base64Converter
{
    // 轉為 Base64（可選是否壓縮、是否 URL-safe）
    public static string ToBase64<T>(this T obj, bool compress = false, bool urlSafe = false)
    {
        string json = JsonConvert.SerializeObject(obj, Formatting.None);
        byte[] jsonBytes = Encoding.UTF8.GetBytes(json);
        if (compress)
        {
            jsonBytes = Compress(jsonBytes);
        }

        string base64 = Convert.ToBase64String(jsonBytes);
        return urlSafe ? ToUrlSafeBase64(base64) : base64;
    }

    // 從 Base64 還原（可選是否壓縮、是否 URL-safe）
    public static T? FromBase64<T>(this string base64, bool compressed = false, bool urlSafe = false)
    {
        if (urlSafe)
        {
            base64 = FromUrlSafeBase64(base64);
        }

        byte[] bytes = Convert.FromBase64String(base64);
        if (compressed)
        {
            bytes = Decompress(bytes);
        }

        string json = Encoding.UTF8.GetString(bytes);
        return JsonConvert.DeserializeObject<T>(json);
    }

    // 壓縮 byte[]
    private static byte[] Compress(byte[] data)
    {
        using var output = new MemoryStream();
        using (var gzip = new GZipStream(output, CompressionLevel.Optimal))
        {
            gzip.Write(data, 0, data.Length);
        }
        return output.ToArray();
    }

    // 解壓縮 byte[]
    private static byte[] Decompress(byte[] data)
    {
        using var input = new MemoryStream(data);
        using var gzip = new GZipStream(input, CompressionMode.Decompress);
        using var output = new MemoryStream();
        gzip.CopyTo(output);
        return output.ToArray();
    }

    // 將 Base64 編碼轉為 URL-safe 版本
    private static string ToUrlSafeBase64(string base64)
    {
        return base64.Replace('+', '-').Replace('/', '_').TrimEnd('=');
    }

    // 將 URL-safe Base64 還原為標準 Base64
    private static string FromUrlSafeBase64(string urlSafeBase64)
    {
        string base64 = urlSafeBase64.Replace('-', '+').Replace('_', '/');
        // 補足尾端的 '='（原本的 Base64 長度必須是4的倍數）
        switch (base64.Length % 4)
        {
            case 2: base64 += "=="; break;
            case 3: base64 += "="; break;
        }
        return base64;
    }
}
