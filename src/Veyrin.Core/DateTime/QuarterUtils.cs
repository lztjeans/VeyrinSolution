public static class QuarterUtils
{
    /// <summary>
    /// 將 YYYYMM 轉成季度版本字串 (e.g., "202403" => "2024Q1")
    /// </summary>
    public static string ToQuarterVersion(string yyyymm)
    {
        return GetYear(yyyymm) + GetQuarterFromMonth(GetMonth(yyyymm));
    }

    /// <summary>
    /// 將年月或版本字串轉成季度版本字串 (e.g., "202403" => "2024Q1")
    /// </summary>
    public static string ConvertQuarter(string version)
    {
        return GetYear(version) + GetQuarterFromMonth(GetMonth(version));
    }

    /// <summary>
    /// 將年份與季度組合成版本字串 (e.g., 2024, "Q1" => "2024Q1")
    /// </summary>
    public static string GetVersion(string year, string quarter) => $"{year}Q{quarter}";
    public static string GetVersion(int year, int quarter) => $"{year}Q{quarter}";
    public static string GetVersion(int year, string quarter) => $"{year}Q{quarter}";
    public static string GetVersion(string year, int quarter) => $"{year}Q{quarter}";

    /// <summary>
    /// 從版本字串取得年份 (YYYY 或 YYYYQx)
    /// </summary>
    public static string GetYear(string version)
    {
        if (string.IsNullOrWhiteSpace(version))
            throw new ArgumentException("Invalid value");

        return version.Contains('Q') ? version.Split('Q')[0] : version[..4];
    }

    /// <summary>
    /// 從季度版本字串取得季度 (Q1~Q4)
    /// </summary>
    public static string GetQuarter(string version)
    {
        if (string.IsNullOrWhiteSpace(version) || !version.Contains('Q'))
            throw new ArgumentException("Invalid value");

        return version.Split('Q')[1];
    }

    /// <summary>
    /// 從 YYYYMM 取得月份 (MM)
    /// </summary>
    public static string GetMonth(string yyyymm)
    {
        if (string.IsNullOrWhiteSpace(yyyymm) || yyyymm.Length != 6)
            throw new ArgumentException("Invalid value");

        return yyyymm.Substring(4, 2);
    }

    /// <summary>
    /// 將月份 (1~12) 轉成季度 (Q1~Q4)
    /// </summary>
    public static string GetQuarterFromMonth(string month)
    {
        return GetQuarterFromMonth(ToInt(month));
    }

    public static string GetQuarterFromMonth(int month)
    {
        return month switch
        {
            1 or 2 or 3 => "Q1",
            4 or 5 or 6 => "Q2",
            7 or 8 or 9 => "Q3",
            10 or 11 or 12 => "Q4",
            _ => throw new ArgumentException("Invalid month value"),
        };
    }

    /// <summary>
    /// 將字串轉成整數，失敗則丟例外
    /// </summary>
    private static int ToInt(string? str)
    {
        if (int.TryParse(str, out int result))
            return result;

        throw new ArgumentException("Invalid int string");
    }

    /// <summary>
    /// 將版本字串拆分成 [年份, 季度]，若非季度版本則回傳空陣列
    /// </summary>
    public static string[] GetVersionParts(string? version)
    {
        return !string.IsNullOrEmpty(version) && version.Contains('Q') ? version.Split('Q') : Array.Empty<string>();
    }
}
