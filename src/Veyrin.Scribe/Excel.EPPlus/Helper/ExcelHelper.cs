using OfficeOpenXml;
using System.Diagnostics.CodeAnalysis;

namespace Veyrin.Scribe.Excel.EPPlus.Helper;

public static class ExcelHelper
{
    // =======================
    // Workbook 初始化防禦
    // =======================
    public static ExcelPackage EnsurePkg([NotNull] this ExcelPackage? package) => package ??
            throw new InvalidOperationException("Workbook has not been initialized. Call Create... or LoadWorkbook first.");
    public static ExcelWorkbook EnsureWorkbook(this ExcelPackage? package) => package.EnsurePkg().Workbook!;
    
    //ExcelPackage package = new ExcelPackage("");
    //ExcelWorkbook wb = package.Workbook;
    //ExcelWorksheet sheet = wb.Worksheets[0];
    //ExcelRangeRow row = sheet.Rows[0];
    //ExcelRange cell = sheet.Cells[1, 1];
    ////row.



    // =======================
    // Worksheet 安全操作
    // =======================
    public static ExcelWorksheet EnsureSheet([NotNull] this ExcelWorksheet? sheet) => sheet ??
        throw new InvalidOperationException("當前沒有作用中的工作表 (Active Sheet)。請先呼叫 AddWorksheet 或 SetActiveWorksheet。");

    public static bool TryGetSheet(this ExcelWorkbook workbook, string sheetName, [MaybeNullWhen(false)] out ExcelWorksheet sheet)
    {
        sheet = workbook.Worksheets
                        .FirstOrDefault(s => s.Name.Equals(sheetName, StringComparison.CurrentCultureIgnoreCase));
        return sheet != null;
    }

    public static ExcelWorksheet GetOrAddSheet(this ExcelWorkbook workbook, string sheetName)
    {
        if(TryGetSheet(workbook, sheetName, out var sheet))
            return sheet;
        else
           return workbook.Worksheets.Add(sheetName);
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
    /// <summary>
    /// 嘗試取得指定列。如果該列完全沒有資料或樣式設定，則回傳 false。
    /// </summary>
    public static bool TryGetRow(this ExcelWorksheet sheet, int rowIndex, [MaybeNullWhen(false)] out ExcelRow row)
    {
        // 檢查該列索引是否在有效範圍內，且該列是否有被使用過的跡象
        if (rowIndex >= 1 && rowIndex <= sheet.Dimension?.End.Row)
        {
            row = sheet.Row(rowIndex);
            return row != null;
        }
        row = null;
        return false;
    }

    /// <summary>
    /// 取得指定列。
    /// </summary>
    public static ExcelRow GetOrAddRow(this ExcelWorksheet sheet, int rowIndex)
    {
        return sheet.Row(rowIndex);
    }

    /// <summary>
    /// 嘗試取得儲存格，若不存在則回傳 false
    /// </summary>
    public static bool TryGetCell(this ExcelWorksheet sheet, int row, int col, [MaybeNullWhen(false)] out ExcelRange cell)
    {
        cell = sheet.Cells[row, col];
        return cell != null && cell.Value != null;
    }

    /// <summary>
    /// 取得儲存格，如果該儲存格尚未初始化或為空，確保它可以被後續操作使用
    /// </summary>
    public static ExcelRange GetOrAddCell(this ExcelWorksheet sheet, int row, int col)
    {
        var cell = sheet.Cells[row, col];
        return cell;
    }



}