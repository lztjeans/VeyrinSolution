using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.SS.Util;
using NPOI.XSSF.UserModel;
using System.Net.Http.Headers;
using Veyrin.Scribe.Core.Interfaces;
using Veyrin.Scribe.Core.Interfaces.Excel;
using Veyrin.Scribe.Core.Models;
using Veyrin.Scribe.NPOI.Helper;

namespace Veyrin.Scribe.NPOI;

public class ExcelEngine : IExcelEngine, IEngine
{
    private IWorkbook? _workbook;
    private ISheet? _activeSheet;
    private int _activeRow;
    private int _activeColumn;

    //===============================
    private IWorkbook Wb => _workbook.EnsureWorkbook();
    private ISheet ActiveSheet => _activeSheet.EnsureSheet();
    //===============================

    private ExcelEngine CreateWorkbook(bool useXls)
    {
        _workbook = useXls ? new HSSFWorkbook() : new XSSFWorkbook();
        return this;
    }
    public IExcelEngine CreateXLSWorkbook() => CreateWorkbook(false);

    public IExcelEngine CreateXLSXWorkbook() => CreateWorkbook(true);

    public IExcelEngine LoadWorkbook(string path)
    {
        using var fs = new FileStream(path, FileMode.Open, FileAccess.Read);
        if (StringUtils.EqualsIgnoreCase(Path.GetExtension(path), ".xls"))
            _workbook = new HSSFWorkbook(fs);
        else
            _workbook = new XSSFWorkbook(fs);
        return this;
    }

    public IExcelEngine SaveToFile(string path)
    {
        using var fs = new FileStream(path, FileMode.Create, FileAccess.Write);
        Wb.Write(fs);
        return this;
    }

    public MemoryStream SaveToStream()
    {
        var ms = new MemoryStream();
        Wb.Write(ms);
        ms.Position = 0;
        return ms;
    }

    public byte[] SaveToByteArray()
    {
        using var ms = SaveToStream();
        return ms.ToArray();
    }

    // ======================= Worksheet 操作 =======================
    public IExcelEngine AddWorksheet(string sheetName)
    {
        ArgumentNullException.ThrowIfNullOrEmpty(sheetName);
        _activeSheet = Wb.GetOrAddSheet(sheetName);
        return this;
    }
    public IExcelEngine DeleteWorksheets(params string[] sheetNames)
    {
        foreach (var sheetName in sheetNames)
        {
            int index = GetSheetIndex(sheetName);
            if (index >= 0) Wb.RemoveSheetAt(index);
        }
        return this;
    }
    public IExcelEngine DeleteWorksheets(params int[] sheetIdx)
    {
        foreach (var index in sheetIdx.Where(x => x >= 0).ToList())
            Wb.RemoveSheetAt(index);
        return this;
    }
    public IExcelEngine CleanAllWorksheets()
    {
        for (int i = Wb.NumberOfSheets - 1; i >= 0; i--)
            Wb.RemoveSheetAt(i);
        return this;
    }
    public IExcelEngine RenameWorksheet(string oldName, string newName)
    {
        int idx = Wb.GetSheetIndex(oldName);
        if (idx >= 0) Wb.SetSheetName(idx, newName);
        return this;
    }
    public IExcelEngine MoveWorksheet(string sheetName, int newIndex)
    {
        int oldIndex = Wb.GetSheetIndex(sheetName);
        if (oldIndex >= 0)
        {
            Wb.SetSheetOrder(sheetName, newIndex - 1);
        }
        return this;
    }
    public IExcelEngine CopyWorksheet(string sourceSheetName, string newSheetName)
    {
        var src = Wb.GetSheet(sourceSheetName);
        if (src != null)
        {
            var copy = Wb.CloneSheet(Wb.GetSheetIndex(src));
            Wb.SetSheetName(Wb.GetSheetIndex(copy), newSheetName);
        }
        return this;
    }
    public IExcelEngine HideWorksheet(string sheetName) => SetSheetHidden(sheetName, SheetVisibility.VeryHidden);
    public IExcelEngine VeryHideWorksheet(string sheetName) => SetSheetHidden(sheetName, SheetVisibility.Hidden);
    public IExcelEngine UnhideWorksheet(string sheetName) => SetSheetHidden(sheetName, SheetVisibility.Visible);
    private ExcelEngine SetSheetHidden(string sheetName, SheetVisibility hidden)
    {
        int idx = Wb.GetSheetIndex(sheetName);
        if (idx >= 0) Wb.SetSheetHidden(idx, hidden);
        return this;
    }
    public Dictionary<int, string> GetWorksheetNames()
    {
        var dict = new Dictionary<int, string>();
        for (int i = 0; i < Wb.NumberOfSheets; i++)
            dict.Add(i + 1, GetSheetName(i));
        return dict;
    }
    public IExcelEngine SetActiveWorksheet(string sheetName)
    {
        if (Wb.TryGetSheet(sheetName, out var sheet))
            _activeSheet = sheet;
        return this;
    }
    public IExcelEngine SetActiveWorksheet(int sheetIndex) => SetActiveWorksheet(GetSheetName(sheetIndex));
    public string? GetActiveWorksheetName() => ActiveSheet.SheetName;

    // =======================/ Row 操作 =======================
    public IExcelEngine SetActiveRow(int row) { _activeRow = row; _activeColumn = 0; return this; }
    public int? GetActiveRow() => _activeRow;
    public IExcelEngine NextRow() { _activeRow++; _activeColumn = 0; return this; }
    public IExcelEngine InsertRow(string sheetName, int row)
    {
        if (Wb.TryGetSheet(sheetName, out var sheet))
            sheet.ShiftRows(row - 1, sheet.LastRowNum, 1);
        return this;
    }
    public IExcelEngine DeleteRow(string sheetName, int row)
    {
        if (Wb.TryGetSheet(sheetName, out var sheet))
            if (sheet.TryGetRow(row--, out var r))
                sheet.RemoveRow(r);
        return this;
    }
    public IExcelEngine InsertRow(int row) => InsertRow(ActiveSheet.SheetName, row);
    public IExcelEngine DeleteRow(int row) => DeleteRow(ActiveSheet.SheetName, row);

    // =======================/ Column 操作 =======================
    public IExcelEngine SetActiveColumn(int col) { _activeColumn = col; return this; }
    public int? GetActiveColumn() => _activeColumn;
    public IExcelEngine NextColumn() { _activeColumn++; return this; }
    public IExcelEngine InsertColumn(string sheetName, int col)
    {
        // NPOI 不直接支援插入欄位，需自行移動每個cell
        if (!Wb.TryGetSheet(sheetName, out ISheet? sheet)) return this;
        foreach (IRow row in sheet)
            for (int c = row.LastCellNum - 1; c >= col - 1; c--)
                if (row.TryGetCell(c, out var oldCell))
                    CloneCell(oldCell, row.CreateCell(c + 1, oldCell.CellType));
        return this;
    }
    public IExcelEngine DeleteColumn(string sheetName, int col)
    {
        if (!Wb.TryGetSheet(sheetName, out ISheet? sheet)) return this;
        foreach (IRow row in sheet)
            for (int c = col - 1; c < row.LastCellNum; c++)
                if (row.TryGetCell(c + 1, out var oldCell))
                    CloneCell(oldCell, row.GetOrAddCell(c));
        return this;
    }
    public IExcelEngine InsertColumn(int col) => InsertColumn(ActiveSheet.SheetName, col);
    public IExcelEngine DeleteColumn(int col) => DeleteColumn(ActiveSheet.SheetName, col);
    // ======================= Cell 操作 =======================
    public IExcelEngine SetCellValue(int row, int column, object value) => SetCellValue(ActiveSheet.SheetName, row, column, value);
    public IExcelEngine SetCellValue(string sheetName, int row, int column, object value)
    {
        if (StringUtils.IsEmpty(sheetName)) sheetName = "Sheet1";
        var c = Wb.GetOrAddSheet(sheetName).GetOrAddRow(row).GetOrAddCell(column);
        c.SetCellValue(value?.ToString() ?? "");
        return this;
    }
    public IExcelEngine SetCellValue(object value)
    {
        var c = ActiveSheet.GetOrAddRow(_activeRow - 1).GetOrAddCell(_activeColumn);
        c.SetCellValue(value?.ToString() ?? "");
        _activeColumn++;
        return this;
    }
    public object GetCellValue(string sheetName, int row, int column)
    {
        var c = Wb.GetOrAddSheet(sheetName).GetOrAddRow(row).GetOrAddCell(column);
        return c?.ToString() ?? "";
    }
    public object GetCellValue(int row, int column) => GetCellValue(ActiveSheet.SheetName, row, column);
    public IExcelEngine MergeCells(string sheetName, int startRow, int startCol, int endRow, int endCol)
    {
        _activeSheet = Wb.GetSheet(sheetName);
        _activeSheet.AddMergedRegion(new CellRangeAddress(startRow - 1, endRow - 1, startCol - 1, endCol - 1));
        return this;
    }
    public IExcelEngine MergeCells(int startRow, int startColumn, int endRow, int endColumn) => MergeCells(ActiveSheet.SheetName, startRow, startColumn, endRow, endColumn);
    public IExcelEngine UnmergeCells(string sheetName, int startRow, int startCol, int endRow, int endCol)
    {
        _activeSheet = Wb.GetSheet(sheetName);
        for (int i = _activeSheet.NumMergedRegions - 1; i >= 0; i--)
        {
            var range = _activeSheet.GetMergedRegion(i);
            if (range.FirstRow == startRow - 1 && range.LastRow == endRow - 1 &&
                range.FirstColumn == startCol - 1 && range.LastColumn == endCol - 1)
                _activeSheet.RemoveMergedRegion(i);
        }
        return this;
    }
    public IExcelEngine UnmergeCells(int startRow, int startColumn, int endRow, int endColumn) => UnmergeCells(ActiveSheet.SheetName, startRow, startColumn, endRow, endColumn);
    public IExcelEngine SetCellStyle(string sheetName, int row, int column, DocumentFontStyle style)
    {
        _activeSheet = Wb.GetSheet(sheetName);
        var r = _activeSheet.GetRow(row - 1) ?? _activeSheet.CreateRow(row - 1);
        var c = r.GetCell(column - 1) ?? r.CreateCell(column - 1);

        ICellStyle cellStyle = Wb.CreateCellStyle();
        var font = Wb.CreateFont();
        //if ((bool)style.Bold) font.IsBold = true;
        font.IsBold = style.Bold;
        if (!string.IsNullOrEmpty(style.FontColor))
        {
            //var color = System.Drawing.ColorTranslator.FromHtml(style.FontColor);
            font.Color = IndexedColors.Black.Index; // NPOI XSSF 直接支援 RGB 需用 SetColor
        }
        cellStyle.SetFont(font);
        c.CellStyle = cellStyle;
        return this;
    }
    public IExcelEngine SetCellStyle(int row, int column, DocumentFontStyle style) => SetCellStyle(ActiveSheet.SheetName, row, column, style);

    // ======================= Native Object =======================
    public object GetNativeWorkbook() => Wb;
    public object GetNativeWorksheet(string sheetName) => Wb.GetSheet(sheetName);
    public object GetNativeRow(string sheetName, int row)
    {
        var ws = Wb.GetSheet(sheetName);
        return ws.GetRow(row - 1);
    }
    public object GetNativeCell(string sheetName, int row, int column)
    {
        ISheet ws = Wb.GetSheet(sheetName);
        return ws!.GetRow(row - 1)!.GetCell(column - 1);
    }

    // ======================= Helper =======================
    private static void CloneCell(ICell source, ICell target)
    {
        target.SetCellType(source.CellType);
        target.SetCellValue(source.ToString());
        target.CellStyle = source.CellStyle;
    }

    private int GetSheetIndex(string name) => Wb.GetSheetIndex(name);
    private string GetSheetName(int index)
    {
        try
        {
            return Wb.GetSheetName(index);
        }
        catch (Exception)
        {
            return "";
        }
    }


}
