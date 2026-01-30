public static class QuarterExpressionParser
{
    /// <summary>
    /// 主解析器：支援 "," 和 "~"
    /// </summary>
    public static List<DateTime> Parse(string input)
    {
        if (string.IsNullOrWhiteSpace(input))
            return [];
        var results = new List<DateTime>();
        var parts = input.Split(',', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);
        foreach (var part in parts)
        {
            if (part.Contains('~'))
                results.AddRange(ParseRange(part));
            else
                results.AddRange(ParseSingle(part));
        }
        return results.Distinct().OrderBy(d => d).ToList();
    }




    // =====================================================================
    // 單個季度或年份項目，例如：2024Q1-F
    // =====================================================================
    private static IEnumerable<DateTime> ParseSingle(string expr)
    {
        expr = expr.Trim().ToUpperInvariant();
        string flag = ExtractFlag(ref expr);

        // 季度模式
        if (expr.Contains('Q'))
            return ExpandQuarter(expr, flag);

        // 年模式
        return ExpandYear(int.Parse(expr), flag);
    }

    private static string ExtractFlag(ref string expr)
    {
        if (!expr.Contains('-')) return "DEFAULT";
        var parts = expr.Split('-');
        expr = parts[0];
        return parts[1];
    }

    private static IEnumerable<DateTime> ExpandQuarter(string yq, string flag)
    {
        var parts = yq.Split('Q');
        int year = int.Parse(parts[0]);
        int quarter = int.Parse(parts[1]);

        int startMonth = (quarter - 1) * 3 + 1;

        return flag switch
        {
            "F" => Enumerable.Range(0, 3)
                .SelectMany(i => ExpandFullMonth(year, startMonth + i)),

            "M" => Enumerable.Range(0, 3)
                .Select(i => new DateTime(year, startMonth + i, 15)),

            "E" => Enumerable.Range(0, 3)
                .Select(i =>
                    new DateTime(year, startMonth + i, DateTime.DaysInMonth(year, startMonth + i))),

            _ => Enumerable.Range(0, 3)
                .Select(i => new DateTime(year, startMonth + i, 1))
        };
    }

    private static IEnumerable<DateTime> ExpandYear(int year, string flag)
    {
        return flag switch
        {
            "F" => Enumerable.Range(1, 12)
                .Select(m => new DateTime(year, m, 1)),

            "M" => Enumerable.Range(1, 12)
                .Select(m => new DateTime(year, m, 15)),

            "E" => Enumerable.Range(1, 12)
                .Select(m => new DateTime(year, m, DateTime.DaysInMonth(year, m))),

            _ => new DateTime[] { new(year, 1, 1) }
        };
    }

    private static IEnumerable<DateTime> ExpandFullMonth(int year, int month)
    {
        return Enumerable.Range(1, DateTime.DaysInMonth(year, month))
                         .Select(day => new DateTime(year, month, day));
    }

    // =====================================================================
    // 區段解析  (e.g., "2024Q1-F ~ 2025Q2-E")
    // =====================================================================
    private static IEnumerable<DateTime> ParseRange(string expr)
    {
        var parts = expr.Split('~', StringSplitOptions.TrimEntries);
        ExtractYearQuarter(parts[0], out int sYear, out int sQuarter);
        ExtractYearQuarter(parts[1], out int eYear, out int eQuarter);

        var result = new List<DateTime>();

        for (int y = sYear; y <= eYear; y++)
        {
            int qStart = y == sYear ? sQuarter : 1;
            int qEnd = y == eYear ? eQuarter : 4;

            for (int q = qStart; q <= qEnd; q++)
                result.AddRange(ParseSingle($"{y}Q{q}"));
        }
        return result;
    }

    private static void ExtractYearQuarter(string expr, out int year, out int quarter)
    {
        expr = expr.Split('-')[0]; // 去掉 flag
        if (expr.Contains("Q"))
        {
            var p = expr.Split('Q');
            year = int.Parse(p[0]);
            quarter = int.Parse(p[1]);
        }
        else
        {
            year = int.Parse(expr);
            quarter = 1;
        }
    }
}
