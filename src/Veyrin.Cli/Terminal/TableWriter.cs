using System.Text;
using Veyrin.Core.Reflection;

namespace Veyrin.Cli.Terminal;

public static class TableWriter
{
    private const int CellPadding = 2;

    public static void Write<T>(IEnumerable<T> items, string title = "") where T : class
    {
        var props = ReflectionCache.GetProperties(typeof(T));
        var headers = props.Select(p => p.Name).ToList();
        var rows = new List<List<string>>();

        // 轉換資料為字串矩陣
        foreach (var item in items)
        {
            rows.Add(props.Select(p => p.GetValue(item)?.ToString() ?? "").ToList());
        }

        // 1. 計算每一欄的最大寬度
        var columnWidths = new int[headers.Count];
        for (int i = 0; i < headers.Count; i++)
        {
            int max = headers[i].Length;
            foreach (var row in rows)
            {
                if (row[i].Length > max) max = row[i].Length;
            }
            columnWidths[i] = max + CellPadding;
        }

        // 2. 開始繪製
        if (!string.IsNullOrEmpty(title)) ConsoleWriter.Header(title);

        PrintDivider(columnWidths);
        PrintRow(headers, columnWidths, AnsiCodes.Cyan + AnsiCodes.Bold); // 標題用青色加粗
        PrintDivider(columnWidths);

        foreach (var row in rows)
        {
            PrintRow(row, columnWidths, AnsiCodes.Reset);
        }
        PrintDivider(columnWidths);
    }

    private static void PrintRow(List<string> columns, int[] widths, string colorCode)
    {
        var sb = new StringBuilder("|");
        for (int i = 0; i < columns.Count; i++)
        {
            var text = columns[i];
            sb.Append(' ').Append(colorCode).Append(text.PadRight(widths[i] - 1)).Append(AnsiCodes.Reset).Append('|');
        }
        Console.WriteLine(sb.ToString());
    }

    private static void PrintDivider(int[] widths)
    {
        var sb = new StringBuilder("+");
        foreach (var w in widths)
        {
            sb.Append(new string('-', w)).Append('+');
        }
        Console.WriteLine(sb.ToString());
    }
}

