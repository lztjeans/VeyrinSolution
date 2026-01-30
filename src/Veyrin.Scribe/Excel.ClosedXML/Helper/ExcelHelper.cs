using ClosedXML.Excel;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Veyrin.Scribe.Core.Models;

namespace Veyrin.Scribe.Excel.ClosedXML.Helper;

public static class ExcelHelper
{

    public static XLWorkbook EnsureWorkbook([NotNull] this XLWorkbook? workbook)
    {
        return workbook ?? throw new InvalidOperationException("Workbook not set.");
    }
    public static string EnsureActiveSheet([NotNull] this string? activeSheetName)
    {
        return activeSheetName ??
        throw new InvalidOperationException("Active worksheet not set.");
    }

    public static XLAlignmentVerticalValues Convert(this VerticalAlignment alignment)
    {
        return alignment switch
        {
            VerticalAlignment.Top => XLAlignmentVerticalValues.Top,
            VerticalAlignment.Bottom => XLAlignmentVerticalValues.Bottom,
            _ => XLAlignmentVerticalValues.Center,
        };
    }
    public static XLAlignmentHorizontalValues Convert(this HorizontalAlignment alignment)
    {
        return alignment switch
        {
            HorizontalAlignment.Center => XLAlignmentHorizontalValues.Center,
            HorizontalAlignment.Right => XLAlignmentHorizontalValues.Right,
            HorizontalAlignment.Justify => XLAlignmentHorizontalValues.Justify,
            _ => XLAlignmentHorizontalValues.Left
        };
    }
}
