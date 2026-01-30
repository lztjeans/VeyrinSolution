using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using System.Diagnostics.CodeAnalysis;

namespace Veyrin.Scribe.OpenXML.Helper;

/// <summary>
/// 提供 OpenXML Excel 處理的輔助工具，包含結構校驗、欄位索引轉換與元素操作。
/// </summary>
public static class ExcelHelper
{
    /// <summary>
    /// 確保 <see cref="SpreadsheetDocument"/> 實例已存在且非空。
    /// </summary>
    /// <param name="document">要檢查的 SpreadsheetDocument 實例。</param>
    /// <returns>非空的 SpreadsheetDocument 實例。</returns>
    /// <exception cref="InvalidOperationException">當實例為 null 時拋出。</exception>
    public static SpreadsheetDocument EnsureDocument([NotNull] this SpreadsheetDocument? document)
    {
        return document ?? throw new InvalidOperationException("SpreadsheetDocument 尚未初始化。請先執行 Create 或 Load 方法。");
    }

    /// <summary>
    /// 確保 <see cref="WorkbookPart"/> 已掛載。
    /// </summary>
    /// <param name="workbookPart">要檢查的 WorkbookPart 實例。</param>
    /// <returns>非空的 WorkbookPart 實例。</returns>
    /// <exception cref="InvalidOperationException">當實例為 null 時拋出，通常代表檔案損毀或結構不正確。</exception>
    public static WorkbookPart EnsureWorkbookPart([NotNull] this WorkbookPart? workbookPart)
    {
        return workbookPart ?? throw new InvalidOperationException("WorkbookPart 為空。這通常代表 Excel 文件結構損毀或尚未正確開啟。");
    }

    /// <summary>
    /// 確保當前有作用中的工作表名稱（非 Null 或空字串）。
    /// </summary>
    /// <param name="activeSheetName">工作表名稱字串。</param>
    /// <returns>確保有效的名稱字串。</returns>
    /// <exception cref="InvalidOperationException">當名稱為空時拋出，提示使用者先設定作用中工作表。</exception>
    [return: NotNull]
    public static string EnsureActiveSheetName([NotNull] this string? activeSheetName)
    {
        if (string.IsNullOrEmpty(activeSheetName))
            throw new InvalidOperationException("當前沒有作用中的工作表 (Active Sheet)。請先呼叫 AddWorksheet 或 SetActiveWorksheet。");
        return activeSheetName;
    }

    /// <summary>
    /// 確保 <see cref="Sheets"/> 集合節點存在於 Workbook 中。
    /// </summary>
    /// <param name="workbookPart">Workbook 的部分節點。</param>
    /// <returns>Sheets 集合節點。</returns>
    /// <exception cref="InvalidOperationException">當找不到 Sheets 集合時拋出。</exception>
    public static Sheets EnsureSheets(this WorkbookPart workbookPart)
    {
        return workbookPart.Workbook.Sheets ?? throw new InvalidOperationException("Excel 結構錯誤：找不到 Sheets 集合節點。");
    }

    /// <summary>
    /// 確保 <see cref="Sheet"/> 節點擁有有效的 Relationship Id。
    /// </summary>
    /// <param name="sheet">工作表定義節點。</param>
    /// <returns>關聯 ID (r:id)。</returns>
    /// <exception cref="InvalidOperationException">當找不到關聯 ID 時拋出。</exception>
    public static string EnsureId([NotNull] this Sheet? sheet)
    {
        if (sheet?.Id?.Value == null)
        {
            throw new InvalidOperationException($"工作表 '{sheet?.Name}' 結構異常，找不到關聯 ID (Relationship ID)。");
        }
        return sheet.Id.Value;
    }

    /// <summary>
    /// 將基於 1 的數字索引轉回 Excel 欄名 (例如: 1 -> "A", 27 -> "AA")。
    /// </summary>
    /// <param name="columnIndex">欄位索引 (從 1 開始)。</param>
    /// <returns>對應的 Excel 欄名字串。</returns>
    public static string GetColumnName(int columnIndex)
    {
        int dividend = columnIndex;
        string columnName = string.Empty;
        while (dividend > 0)
        {
            int modifier = (dividend - 1) % 26;
            columnName = Convert.ToChar(65 + modifier).ToString() + columnName;
            dividend = (dividend - modifier) / 26;
        }
        return columnName;
    }

    /// <summary>
    /// 從儲存格參照字串中解析出欄索引 (例如: "B10" -> 2)。
    /// </summary>
    /// <param name="cellReference">儲存格參照字串 (如 "A1", "BC20")。</param>
    /// <returns>欄位索引 (從 1 開始)。</returns>
    public static int GetColumnIndex(string cellReference)
    {
        string columnName = new(cellReference.Where(char.IsLetter).ToArray());
        int index = 0;
        foreach (char c in columnName)
        {
            index *= 26;
            index += c - 'A' + 1;
        }
        return index;
    }

    /// <summary>
    /// 在指定的 <see cref="SheetData"/> 中獲取特定索引的資料列，若不存在則建立。
    /// 該方法會確保資料列按順序插入以維持 Excel 結構合法性。
    /// </summary>
    /// <param name="sd">工作表數據節點。</param>
    /// <param name="rowIndex">目標資料列索引 (從 1 開始)。</param>
    /// <returns>對應的 <see cref="Row"/> 物件。</returns>
    public static Row GetOrAddRow(SheetData sd, uint rowIndex)
    {
        Row? row = sd.Elements<Row>().FirstOrDefault(r => r.RowIndex != null && r.RowIndex.Value == rowIndex);

        if (row != null) return row;
        row = new Row() { RowIndex = rowIndex };

        // 為了防止 Excel 損壞，確保 Row 是按順序插入的
        Row? successor = sd.Elements<Row>().FirstOrDefault(r => r.RowIndex != null && r.RowIndex.Value > rowIndex);
        if (successor != null)
            sd.InsertBefore(row, successor);
        else
            sd.Append(row);

        return row;
    }

    /// <summary>
    /// 在指定的資料列中獲取特定參照的儲存格，若不存在則建立。
    /// </summary>
    /// <param name="row">父資料列物件。</param>
    /// <param name="cellReference">儲存格參照 (如 "A1")。</param>
    /// <returns>對應的 <see cref="Cell"/> 物件。</returns>
    public static Cell GetOrAddCell(Row row, string cellReference)
    {
        Cell? cell = row.Elements<Cell>().FirstOrDefault(c => c.CellReference == cellReference);
        if (cell != null) return cell;
        cell = new Cell() { CellReference = cellReference };
        row.Append(cell);
        return cell;
    }

    /// <summary>
    /// 從儲存格參照解析欄索引 (別名方法，內部呼叫 <see cref="GetColumnNameFromRef"/>)。
    /// </summary>
    /// <param name="cellRef">儲存格參照字串。</param>
    /// <returns>欄位索引 (A=1, B=2...)。</returns>
    public static int GetColumnIndexFromRef(string cellRef)
    {
        string colName = GetColumnNameFromRef(cellRef);
        int index = 0;
        foreach (char c in colName)
        {
            index = index * 26 + c - 'A' + 1;
        }
        return index;
    }

    /// <summary>
    /// 從儲存格參照中提取欄位英文字母部分 (例如: "AB12" -> "AB")。
    /// </summary>
    /// <param name="cellRef">儲存格參照字串。</param>
    /// <returns>欄位英文字母字串。</returns>
    public static string GetColumnNameFromRef(string cellRef) => new(cellRef.Where(char.IsLetter).ToArray());
}