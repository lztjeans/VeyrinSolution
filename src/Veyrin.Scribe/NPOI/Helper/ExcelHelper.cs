using NPOI.SS.UserModel;
using System.Diagnostics.CodeAnalysis;

namespace Veyrin.Scribe.NPOI.Helper;

public static class ExcelHelper
{
    // =======================
    // Workbook 初始化防禦
    // =======================
    public static IWorkbook EnsureWorkbook([NotNull]this IWorkbook? workbook) => workbook ??
            throw new InvalidOperationException("Workbook has not been initialized. Call Create... or LoadWorkbook first.");


    // =======================
    // Worksheet 安全操作
    // =======================

    

    public static ISheet EnsureSheet([NotNull] this ISheet? sheet) => sheet ??
        throw new InvalidOperationException("當前沒有作用中的工作表 (Active Sheet)。請先呼叫 AddWorksheet 或 SetActiveWorksheet。");

    public static bool TryGetSheet(this IWorkbook? workbook, string sheetName, out ISheet sheet)
    {
        sheet = workbook.EnsureWorkbook().GetSheet(sheetName);
        return sheet != null;
    }

    public static ISheet GetOrAddSheet(this IWorkbook? workbook, string sheetName)
    {
        workbook.EnsureWorkbook();
        return workbook.GetSheet(sheetName) ?? workbook.CreateSheet(sheetName);
    }

    ////public static ISheet GetSafeSheet(this IWorkbook? workbook, int index)
    ////{
    ////    workbook.EnsureWorkbook();
    ////    if (index < 0 || index >= workbook.NumberOfSheets)
    ////        return workbook.NumberOfSheets > 0 ? workbook.GetSheetAt(0) : workbook.CreateSheet("Sheet1");
    ////    return workbook.GetSheetAt(index);
    ////}

    // =======================
    // Row / Cell 安全操作 (核心防禦)
    // =======================

    //public static IRow EnsureRow([NotNull] this IRow? row)
    //{
    //    ArgumentNullException.ThrowIfNull(row);
    //    return row;
    //}
    //public static ICell EnsureCell([NotNull] this ICell? row)
    //{
    //    ArgumentNullException.ThrowIfNull(row);
    //    return row;
    //}

    public static bool TryGetRow(this ISheet sheet, int rowIndex, out IRow row)
    {
        row = sheet.GetRow(rowIndex);
        return row != null;
    }

    public static IRow GetOrAddRow(this ISheet? sheet, int row1Based)
    {
        ArgumentNullException.ThrowIfNull(sheet);
        int rowIndex = Math.Max(0, row1Based - 1);
        return sheet.GetRow(rowIndex) ?? sheet.CreateRow(rowIndex);
    }

    //public static bool TryGetCell(this ISheet? sheet, int rowIndex, int cellIndex, out ICell cell)
    //{
    //    cell = null!;
    //    if (!sheet.TryGetRow(rowIndex, out var row))
    //        return false;
    //    cell = row.GetCell(cellIndex);
    //    return cell != null;
    //}
    public static bool TryGetCell(this IRow row, int cellIndex, out ICell cell)
    {
        //cell = null!;
        //if (!sheet.TryGetRow(rowIndex, out var row))
        //    return false;
        cell = row.GetCell(cellIndex);
        return cell != null;
    }


    public static ICell GetOrAddCell(this IRow? row, int col1Based)
    {
        ArgumentNullException.ThrowIfNull(row);
        int colIndex = Math.Max(0, col1Based - 1);
        return row.GetCell(colIndex) ?? row.CreateCell(colIndex);
    }



}
