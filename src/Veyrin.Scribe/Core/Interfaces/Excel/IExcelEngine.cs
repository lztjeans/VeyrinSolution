using Veyrin.Scribe.Core.Models;
namespace Veyrin.Scribe.Core.Interfaces.Excel;
/// <summary>
/// 定義 Excel 操作引擎的共用介面。
/// 可由 ClosedXML、EPPlus、NPOI 等具體引擎實作。
/// </summary>
public interface IExcelEngine
{
    // =============================
    //  Workbook 基本操作
    // =============================
    /// <summary>建立新的工作簿。</summary>
    IExcelEngine CreateXLSWorkbook();
    /// <summary>建立新的工作簿。</summary>
    IExcelEngine CreateXLSXWorkbook();
    /// <summary>從指定檔案載入工作簿。</summary>
    IExcelEngine LoadWorkbook(string path);
    /// <summary>儲存工作簿至指定檔案。</summary>
    IExcelEngine SaveToFile(string path);
    // =============================
    //  Worksheet 操作
    // =============================
    /// <summary>新增工作表。</summary>
    IExcelEngine AddWorksheet(string sheetName);
    /// <summary>刪除工作表。</summary>
    IExcelEngine DeleteWorksheet(string sheetName);
    /// <summary>刪除多個工作表。</summary>
    IExcelEngine DeleteWorksheets(params string[] sheetNames);
    /// <summary>清除所有工作表。</summary>
    IExcelEngine CleanAllWorksheets();
    /// <summary>重新命名工作表。</summary>
    IExcelEngine RenameWorksheet(string oldName, string newName);
    /// <summary>移動工作表至指定索引。</summary>
    IExcelEngine MoveWorksheet(string sheetName, int newIndex);
    /// <summary>複製工作表。</summary>
    IExcelEngine CopyWorksheet(string sourceSheetName, string newSheetName);
    /// <summary>隱藏指定工作表。</summary>
    IExcelEngine HideWorksheet(string sheetName);
    /// <summary>隱藏指定工作表。</summary>
    IExcelEngine VeryHideWorksheet(string sheetName);
    /// <summary>取消隱藏指定工作表。</summary>
    IExcelEngine UnhideWorksheet(string sheetName);
    /// <summary>取得所有工作表名稱。</summary>
    Dictionary<int, string> GetWorksheetNames();
    /// <summary>設定目前作用中工作表。</summary>
    IExcelEngine SetActiveWorksheet(string sheetName);
    /// <summary>取得目前作用中工作表名稱。</summary>
    string? GetActiveWorksheetName();
    // =============================
    //  Row / Column 狀態
    // =============================
    /// <summary>設定目前作用中的列。</summary>
    IExcelEngine SetActiveRow(int row);
    /// <summary>取得目前作用中的列號。</summary>
    int? GetActiveRow();
    /// <summary>設定目前作用中的欄。</summary>
    IExcelEngine SetActiveColumn(int column);
    /// <summary>取得目前作用中的欄號。</summary>
    int? GetActiveColumn();
    /// <summary>跳到下一列（自動重設欄為1）。</summary>
    IExcelEngine NextRow();
    /// <summary>跳到下一欄。</summary>
    IExcelEngine NextColumn();
    // =============================
    //  Cell 操作（指定 sheetName）
    // =============================
    /// <summary>設定儲存格內容。</summary>
    IExcelEngine SetCellValue(string sheetName, int row, int column, object value);
    /// <summary>取得儲存格內容。</summary>
    object? GetCellValue(string sheetName, int row, int column);
    /// <summary>合併儲存格。</summary>
    IExcelEngine MergeCells(string sheetName, int startRow, int startColumn, int endRow, int endColumn);
    /// <summary>設定儲存格樣式。</summary>
    IExcelEngine SetCellStyle(string sheetName, int row, int column, DocumentFontStyle style);
    /// <summary>插入列。</summary>
    IExcelEngine InsertRow(string sheetName, int row);
    /// <summary> 刪除指定一列。 </summary>
    IExcelEngine DeleteRow(string sheetName, int row);
    /// <summary>插入欄。</summary>
    IExcelEngine InsertColumn(string sheetName, int column);
    /// <summary>刪除欄。</summary>
    IExcelEngine DeleteColumn(string sheetName, int column);
    // =============================
    //  Cell 操作（作用中 Sheet）
    // =============================
    /// <summary>設定儲存格內容（作用中 Sheet）。</summary>
    IExcelEngine SetCellValue(int row, int column, object value);
    /// <summary>取得儲存格內容（作用中 Sheet）。</summary>
    object? GetCellValue(int row, int column);
    /// <summary>合併儲存格（作用中 Sheet）。</summary>
    IExcelEngine MergeCells(int startRow, int startColumn, int endRow, int endColumn);
    /// <summary>取消合併指定範圍的儲存格。</summary>
    IExcelEngine UnmergeCells(string sheetName, int startRow, int startColumn, int endRow, int endColumn);
    /// <summary>設定儲存格樣式（作用中 Sheet）。</summary>
    IExcelEngine SetCellStyle(int row, int column, DocumentFontStyle style);
    /// <summary>插入列（作用中 Sheet）。</summary>
    IExcelEngine InsertRow(int row);
    /// <summary>刪除列（作用中 Sheet）。</summary>
    IExcelEngine DeleteRow(int row);
    /// <summary>插入欄（作用中 Sheet）。</summary>
    IExcelEngine InsertColumn(int column);
    /// <summary>刪除欄（作用中 Sheet）。</summary>
    IExcelEngine DeleteColumn(int column);
    // =============================
    //  Cell 操作（作用中 Sheet + Row/Column 狀態）
    // =============================
    /// <summary>設定儲存格內容，使用目前作用中 Sheet / Row / Column，自動往右遞增。</summary>
    IExcelEngine SetCellValue(object value);
    // =============================
    //  批次資料處理 (Data Binding)
    // =============================

    /// <summary>將 DataTable 直接匯入指定工作表。</summary>
    /// <param name="dt">來源資料表</param>
    /// <param name="printHeaders">是否輸出欄位名稱作為標題</param>
    IExcelEngine ImportDataTable(string sheetName, int startRow, int startColumn, System.Data.DataTable dt, bool printHeaders = true);

    /// <summary>自動調整欄寬（根據內容長度）。</summary>
    IExcelEngine AutoFitColumn(string sheetName, int column);
    IExcelEngine AutoFitColumns(string sheetName, int startColumn, int endColumn);

    /// <summary>取得指定工作表的最後一列列號（用於接續寫入資料）。</summary>
    int GetLastRowIndex(string sheetName);
    // =============================
    //  範圍與進階樣式 (Range & Styling)
    // =============================

    /// <summary>設定區域範圍的樣式 (例如 "A1:D10")。</summary>
    IExcelEngine SetRangeStyle(string sheetName, int startRow, int startColumn, int endRow, int endColumn, DocumentFontStyle style);

    /// <summary>設定欄位寬度。</summary>
    IExcelEngine SetColumnWidth(string sheetName, int column, double width);

    /// <summary>設定列高度。</summary>
    IExcelEngine SetRowHeight(string sheetName, int row, double height);

    // =============================
    //  框線操作 (Borders)
    // =============================
    /// <summary>設定儲存格框線（作用中 Sheet）。</summary>
    IExcelEngine SetCellBorder(int row, int column, BorderSettings settings);

    /// <summary>設定範圍框線（作用中 Sheet）。</summary>
    /// <param name="insideLines">是否包含範圍內部的網格線</param>
    IExcelEngine SetRangeBorder(int startRow, int startColumn, int endRow, int endColumn, BorderSettings settings, bool insideLines = false);

    /// <summary>快速設定外框粗線，內部細線（常用於報表外框）（作用中 Sheet）。</summary>
    IExcelEngine SetTableBorder(int startRow, int startColumn, int endRow, int endColumn);
    // =============================
    //  進階功能 (Formulas & View)
    // =============================

    /// <summary>設定儲存格公式 (例如 "=SUM(A1:A10)")。</summary>
    IExcelEngine SetCellFormula(string sheetName, int row, int column, string formula);

    /// <summary>凍結窗格。</summary>
    /// <param name="row">從第幾列開始凍結</param>
    /// <param name="column">從第幾欄開始凍結</param>
    IExcelEngine FreezePanes(string sheetName, int row, int column);

    /// <summary>加上自動篩選按鈕 (AutoFilter)。</summary>
    IExcelEngine SetAutoFilter(string sheetName, int startRow, int startColumn, int endRow, int endColumn);

    /// <summary>插入圖片。</summary>
    IExcelEngine InsertImage(string sheetName, byte[] imageBytes, int row, int column, int width, int height);
    // =============================
    //  搜尋功能 (Search)
    // =============================

    /// <summary>在工作表中搜尋特定內容，回傳其座標 (Row, Column)。</summary>
    /// <returns>若找不到則回傳 null</returns>
    Tuple<int, int>? FindCellValue(string sheetName, string searchText);
    // =============================
    //  進階功能 (作用中 Sheet)
    // =============================

    /// <summary>設定儲存格公式（作用中 Sheet）。</summary>
    IExcelEngine SetCellFormula(int row, int column, string formula);

    /// <summary>凍結窗格（作用中 Sheet）。</summary>
    IExcelEngine FreezePanes(int row, int column);

    /// <summary>加上自動篩選按鈕（作用中 Sheet）。</summary>
    IExcelEngine SetAutoFilter(int startRow, int startColumn, int endRow, int endColumn);

    /// <summary>插入圖片（作用中 Sheet）。</summary>
    IExcelEngine InsertImage(byte[] imageBytes, int row, int column, int width, int height);

    // =============================
    //  搜尋與定位 (作用中 Sheet)
    // =============================

    /// <summary>在目前工作表中搜尋特定內容。</summary>
    Tuple<int, int>? FindCellValue(string searchText);

    /// <summary>自動調整目前工作表指定欄寬。</summary>
    IExcelEngine AutoFitColumn(int column);

    /// <summary>自動調整目前工作表範圍欄寬。</summary>
    IExcelEngine AutoFitColumns(int startColumn, int endColumn);

    /// <summary>取得目前工作表的最後一列列號。</summary>
    int GetLastRowIndex();

    // =============================
    //  範圍與樣式 (作用中 Sheet)
    // =============================

    /// <summary>設定目前工作表區域範圍的樣式。</summary>
    IExcelEngine SetRangeStyle(int startRow, int startColumn, int endRow, int endColumn, DocumentFontStyle style);

    /// <summary>設定目前工作表欄位寬度。</summary>
    IExcelEngine SetColumnWidth(int column, double width);

    /// <summary>設定目前工作表列高度。</summary>
    IExcelEngine SetRowHeight(int row, double height);
    // =============================
    //  原生物件存取
    // =============================
    /// <summary>取得原生 Workbook 物件（EPPlus: ExcelPackage, ClosedXML: XLWorkbook...）。</summary>
    object GetNativeWorkbook();
    /// <summary>取得原生 Worksheet 物件。</summary>
    object GetNativeWorksheet(string sheetName);
    /// <summary>取得原生 Row 物件。</summary>
    object GetNativeRow(string sheetName, int row);
    /// <summary>取得原生 Cell 物件。</summary>
    object GetNativeCell(string sheetName, int row, int column);
}