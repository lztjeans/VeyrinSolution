using System.Globalization;
using System.Text.RegularExpressions;

public static class DateQuarterUtils
{


    // ========================================================================
    //  進階解析：單一年或季度（支援 F / M / E）
    // ========================================================================

    /// <summary>解析單個年份或季度並依 F/M/E 展開成日期序列</summary>
    private static IEnumerable<DateTime> ParseSingleItem(string expr)
    {
        expr = expr.Trim().ToUpperInvariant();
        string flag = "DEFAULT";

        // 處理 flag（如 "2024Q1-F"）
        if (expr.Contains('-'))
        {
            var parts = expr.Split('-');
            expr = parts[0];
            flag = parts[1];
        }

        // -------------------------
        // 解析季度 YYYYQX
        // -------------------------
        if (expr.Contains("Q"))
        {
            var yq = expr.Split('Q');
            int year = int.Parse(yq[0]);
            int quarter = int.Parse(yq[1]);
            int startMonth = (quarter - 1) * 3 + 1;

            return flag switch
            {
                "F" => Enumerable.Range(0, 3).SelectMany(i =>
                            Enumerable.Range(1, DateTime.DaysInMonth(year, startMonth + i))
                                      .Select(d => new DateTime(year, startMonth + i, d))),

                "M" => Enumerable.Range(0, 3)
                                 .Select(i => new DateTime(year, startMonth + i, 15)),

                "E" => Enumerable.Range(0, 3)
                                 .Select(i => new DateTime(year, startMonth + i,
                                            DateTime.DaysInMonth(year, startMonth + i))),

                _ => Enumerable.Range(0, 3)
                                 .Select(i => new DateTime(year, startMonth + i, 1))
            };
        }

        // -------------------------
        // 解析年份 YYYY
        // -------------------------
        {
            int year = int.Parse(expr);
            return flag switch
            {
                "F" => Enumerable.Range(1, 12).Select(m => new DateTime(year, m, 1)),
                "M" => Enumerable.Range(1, 12).Select(m => new DateTime(year, m, 15)),
                "E" => Enumerable.Range(1, 12).Select(m => new DateTime(year, m,
                                            DateTime.DaysInMonth(year, m))),
                _ => new[] { new DateTime(year, 1, 1) }
            };
        }
    }


    // ========================================================================
    //  高階解析：支援 , 與 ~ 混合（季度版本）
    // ========================================================================

    /// <summary>支援範圍 "~" 與逗號 "," 的季度解析</summary>
    public static List<DateTime> ParseQuarterExpressions(string input)
    {
        var result = new List<DateTime>();
        if (string.IsNullOrWhiteSpace(input))
            return result;

        var parts = input.Split(',', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);

        foreach (var part in parts)
        {
            if (part.Contains('~'))
            {
                // 區段解析
                var range = part.Split('~', StringSplitOptions.TrimEntries);
                if (range.Length != 2) continue;

                var startExpr = range[0];
                var endExpr = range[1];

                ExtractYearQuarter(startExpr, out int startYear, out int startQ);
                ExtractYearQuarter(endExpr, out int endYear, out int endQ);

                for (int y = startYear; y <= endYear; y++)
                {
                    int qStart = (y == startYear) ? startQ : 1;
                    int qEnd = (y == endYear) ? endQ : 4;

                    for (int q = qStart; q <= qEnd; q++)
                    {
                        string flag = "DEFAULT";

                        if (y == startYear && q == startQ && startExpr.Contains('-'))
                            flag = startExpr.Split('-')[1];
                        else if (y == endYear && q == endQ && endExpr.Contains('-'))
                            flag = endExpr.Split('-')[1];

                        result.AddRange(ParseSingleItem($"{y}Q{q}-{flag}"));
                    }
                }
            }
            else
            {
                result.AddRange(ParseSingleItem(part));
            }
        }

        return result.Distinct().OrderBy(d => d).ToList();
    }

    private static void ExtractYearQuarter(string expr, out int year, out int quarter)
    {
        quarter = 1;

        if (expr.Contains("Q"))
        {
            var parts = expr.Split('Q');
            year = int.Parse(parts[0]);
            quarter = int.Parse(parts[1][0].ToString());
        }
        else
        {
            year = int.Parse(expr.Split('-')[0]);
        }
    }


    // ========================================================================
    //  日期解析（支援日/月/年 + F/M/E/F-F）
    // ========================================================================

    public static List<DateTime> ParseDates(string inputDates)
    {
        var list = new List<DateTime>();
        if (string.IsNullOrWhiteSpace(inputDates))
            return list;

        var parts = inputDates.Split(',', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);

        foreach (var part in parts)
        {
            if (!part.Contains('~'))
            {
                list.AddRange(ParseSingleExpression(part));
                continue;
            }

            // 處理 yyyy... ~ yyyy...
            var range = part.Split('~', StringSplitOptions.TrimEntries);
            if (range.Length != 2) continue;

            string startExpr = range[0];
            string endExpr = range[1];

            int startYear = ExtractYear(startExpr);
            int endYear = ExtractYear(endExpr);

            for (int y = startYear; y <= endYear; y++)
            {
                string expr;

                if (y == startYear && y == endYear)
                    expr = startExpr;
                else if (y == startYear)
                    expr = startExpr;
                else if (y == endYear)
                    expr = endExpr;
                else
                    expr = Regex.Replace(startExpr, @"^\d{4}", y.ToString());

                list.AddRange(ParseSingleExpression(expr));
            }
        }

        return list.Distinct().OrderBy(d => d).ToList();
    }

    private static int ExtractYear(string expr)
        => int.Parse(Regex.Match(expr, @"^\d{4}").Value);

    private static IEnumerable<DateTime> ParseSingleExpression(string expr)
    {
        expr = expr.Trim().ToUpperInvariant();

        // yyyy-MM-dd
        if (DateTime.TryParseExact(expr, "yyyy-MM-dd", CultureInfo.InvariantCulture,
                                   DateTimeStyles.None, out var dt))
            return [dt];

        // 月級（yyyy-MM / yyyy-MM-M/E/F）
        var monthPattern = @"^(?<y>\d{4})-(?<m>\d{2})(-(?<flag>[MEF]))?$";
        var mm = Regex.Match(expr, monthPattern);
        if (mm.Success)
        {
            int year = int.Parse(mm.Groups["y"].Value);
            int month = int.Parse(mm.Groups["m"].Value);
            string flag = mm.Groups["flag"].Value.ToUpperInvariant();

            var first = new DateTime(year, month, 1);

            return flag switch
            {
                "M" => [first.AddDays(14)],
                "E" => [new DateTime(year, month, DateTime.DaysInMonth(year, month))],
                "F" => Enumerable.Range(0, DateTime.DaysInMonth(year, month))
                                 .Select(d => first.AddDays(d)),
                _ => [first]
            };
        }

        // 年級 yyyy / yyyy-F / yyyy-M / yyyy-E / yyyy-F-F
        var yearPattern = @"^(?<y>\d{4})(-(?<flag>F|F-F|M|E))?$";
        var yy = Regex.Match(expr, yearPattern);

        if (yy.Success)
        {
            int year = int.Parse(yy.Groups["y"].Value);
            string flag = yy.Groups["flag"].Value.ToUpperInvariant();

            return flag switch
            {
                "F" =>
                    Enumerable.Range(1, 12).Select(m => new DateTime(year, m, 1)),

                "F-F" =>
                    Enumerable.Range(0, (new DateTime(year, 12, 31) -
                                         new DateTime(year, 1, 1)).Days + 1)
                              .Select(d => new DateTime(year, 1, 1).AddDays(d)),

                "M" =>
                    Enumerable.Range(1, 12).Select(m => new DateTime(year, m, 15)),

                "E" =>
                    Enumerable.Range(1, 12).Select(m => new DateTime(year, m,
                                        DateTime.DaysInMonth(year, m))),

                _ =>
                    [new DateTime(year, 1, 1)]
            };
        }

        return [];
    }
}
