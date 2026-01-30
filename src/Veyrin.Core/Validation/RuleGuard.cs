using System.Runtime.CompilerServices;
using Veyrin.Core.Exceptions;

namespace Veyrin.Core.Validation;

public static partial class Guard
{
    // URL 驗證
    public static void IsUrl(this string? input, [CallerArgumentExpression("input")] string paramName = "")
    {
        const string pattern = @"^(https?|ftp):\/\/[^\s/$.?#].[^\s]*$";
        if (!StringUtils.IsMatch(input ?? "", pattern, out _))
            throw new ValidationException($"{paramName} is not a valid URL.", paramName);
    }

    // Guid 驗證
    public static void IsGuid(this string? input, [CallerArgumentExpression("input")] string paramName = "")
    {
        if (!Guid.TryParse(input, out _))
            throw new ValidationException($"{paramName} must be a valid GUID.", paramName);
    }

    // Base64 驗證
    public static void IsBase64(this string? input, [CallerArgumentExpression("input")] string paramName = "")
    {
        if (StringUtils.IsEmpty(input) || input.Length % 4 != 0 || !StringUtils.IsMatch(input, @"^[a-zA-Z0-9\+/]*={0,2}$", out _))
            throw new ValidationException($"{paramName} is not a valid Base64 string.", paramName);
    }
    /// <summary>
    /// 驗證字串是否符合 Email 格式
    /// </summary>
    /// <param name="input">待檢查的字串</param>
    /// <param name="paramName">參數名稱，用於錯誤訊息</param>
    public static void IsEmail(this string? input, [CallerArgumentExpression("input")] string paramName = "")
    {
        // 1. 先進行基礎的 Null 或空值檢查 (引用我們剛才建立的 NotEmpty)
        Guard.NotEmpty(input, paramName);

        // 2. 定義 Email 正則表達式 (這是一個相對穩定且通用的模式)
        const string emailPattern = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";

        // 3. 呼叫您的 StringUtils 工具類別進行匹配
        // 注意：這裡假設您的 IsMatch 會回傳 bool 且包含 Regex 邏輯
        if (!StringUtils.IsMatch(input!, emailPattern, out _))
            throw new ValidationException($"{paramName} is not a valid email format.", paramName);
    }

    /// <summary>
    /// 檢查檔名或路徑是否包含系統非法字元
    /// </summary>
    public static void IsValidPath(this string? path, [CallerArgumentExpression("path")] string paramName = "")
    {
        Guard.NotEmpty(path, paramName);

        // 取得系統定義的非法路徑字元
        var invalidChars = Path.GetInvalidPathChars();
        if (path!.Any(c => invalidChars.Contains(c)))
        {
            throw new ValidationException($"{paramName} contains invalid path characters.", paramName);
        }
    }

    /// <summary>
    /// 確保副檔名符合預期 (例如: .json, .txt)
    /// </summary>
    public static void HasExtension(this string? fileName, string extension, [CallerArgumentExpression("fileName")] string paramName = "")
    {
        Guard.NotEmpty(fileName, paramName);
        if(StringUtils.EndsWith(fileName, extension))
            throw new ValidationException($"{paramName} must have a {extension} extension.", paramName);
    }

    /// <summary>
    /// 驗證是否為有效的 IP 位址 (支援 IPv4 與 IPv6)
    /// </summary>
    public static void IsIpAddress(this string? ip, [CallerArgumentExpression("ip")] string paramName = "")
    {
        if (!System.Net.IPAddress.TryParse(ip, out _))
            throw new ValidationException($"{paramName} is not a valid IP address.", paramName);
    }

    /// <summary>
    /// 驗證連接埠號 (Port) 是否在有效範圍 (1-65535)
    /// </summary>
    public static void IsPort(this int port, [CallerArgumentExpression("port")] string paramName = "")
    {
        // 複用我們先前完成的 InRange
        port.InRange(1, 65535, paramName);
    }

    /// <summary>
    /// 簡單驗證字串是否為 JSON 格式 (以 { 或 [ 開始與結束)
    /// </summary>
    public static void IsJson(this string? input, [CallerArgumentExpression("input")] string paramName = "")
    {
        Guard.NotEmpty(input, paramName);
        var trimmed = input!.Trim();
        bool isJson = (StringUtils.StartsWith(trimmed,"{") && StringUtils.EndsWith(trimmed,"}")) ||
                      (StringUtils.StartsWith(trimmed,"[") && StringUtils.EndsWith(trimmed,"]"));

        if (!isJson)
            throw new ValidationException($"{paramName} is not a valid JSON string structure.", paramName);
    }

    /// <summary>
    /// 驗證 Slug 格式 (僅限小寫字母、數字與連字號，常用於網址路徑)
    /// </summary>
    public static void IsSlug(this string? input, [CallerArgumentExpression("input")] string paramName = "")
    {
        const string pattern = @"^[a-z0-9]+(?:-[a-z0-9]+)*$";
        if (!StringUtils.IsMatch(input ?? "", pattern, out _))
            throw new ValidationException($"{paramName} must be a valid slug (lowercased, hyphenated).", paramName);
    }

}
