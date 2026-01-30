public class DateTimeUtils
{
    /// <summary>
    /// 從月份字串轉季度 (e.g., "03" => "Q1")
    /// </summary>
    public static string GetQuarterFromMonth(string month) => GetQuarterFromMonth(month.ToNumber<int>());

    /// <summary>
    /// 從月份整數轉季度 (e.g., 3 => "Q1")
    /// </summary>
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
    /// 從季度版本字串拆解成 [年份, 季度] (e.g., "2024Q1" => ["2024", "1"])
    /// </summary>
    public static string[] SplitYearQuarter(string version) =>
        !string.IsNullOrWhiteSpace(version) && version.Contains('Q') ? version.Split('Q') : [];


    /// <summary>
    /// 取得年份 (e.g., "2024Q1" => "2024", "202403" => "2024")
    /// </summary>
    [Obsolete]
    public static string GetYear1(string version)
    {
        if (string.IsNullOrWhiteSpace(version)) throw new ArgumentException("Invalid value");
        return version.Contains('Q') ? SplitYearQuarter(version)[0] : version[..4];
    }
    public static string GetYear(string version)
    {
        if (string.IsNullOrWhiteSpace(version)) throw new ArgumentException("Invalid value");
        return version.Contains('Q') ? SplitYearQuarter(version)[0] : version[..4];
    }

    public static string GetMonth(string yyyymm)
    {
        if (string.IsNullOrWhiteSpace(yyyymm) || yyyymm.Length != 6)
            throw new ArgumentException("Invalid value");
        return yyyymm[4..6];
    }
    /// <summary>
    /// 取得月份 (僅支援 YYYYMM 格式, e.g., "202403" => "03")
    /// </summary>
    [Obsolete]
    public static string GetMonth1(string version)
    {
        if (string.IsNullOrWhiteSpace(version)) throw new ArgumentException("Invalid value");
        if (version.Length != 6) throw new ArgumentException("Invalid value");
        return version.Substring(4, 2);
    }




}
