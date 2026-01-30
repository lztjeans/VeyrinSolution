using OfficeOpenXml;
using OfficeOpenXml.Style;
using Veyrin.Scribe.Core.Interfaces;
using Veyrin.Scribe.Core.Interfaces.Excel;
using Veyrin.Scribe.Core.Models;
using Veyrin.Scribe.Excel.EPPlus.Helper;

namespace Veyrin.Scribe.Excel.EPPlus;

public class ExcelEngine : IExcelEngine, IEngine
{
    static ExcelEngine()
    {
        //ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
        ExcelPackage.License.SetCommercial("<Your License Key here>");
        //ExcelPackage.License.SetLicense(OfficeOpenXml.LicenseStyle.NonCommercial);
        ExcelPackage.License.SetNonCommercialPersonal("<Your Name>");
    }

    private ExcelPackage? _package;
    private ExcelWorksheet? _activeSheet;
    private int _activeRow = 1;
    private int _activeColumn = 1;


    private ExcelWorkbook Wb => _package.EnsureWorkbook();
    private ExcelWorksheet ActiveSheet => _activeSheet.EnsureSheet();

    public IExcelEngine CreateXLSWorkbook() => throw new NotSupportedException("EPPlus 僅支援 .xlsx 格式。");

    public IExcelEngine CreateXLSXWorkbook()
    {
        _package = new ExcelPackage();
        return this;
    }
    public IExcelEngine LoadWorkbook(string path)
    {
        var file = new FileInfo(path);
        _package = new ExcelPackage(file);
        return this;
    }

    public IExcelEngine SaveToFile(string path)
    {
        _package.EnsurePkg().SaveAs(new FileInfo(path));
        return this;
    }

    public MemoryStream SaveToStream()
    {
        var ms = new MemoryStream();
        _package.EnsurePkg().SaveAs(ms);
        ms.Position = 0;
        return ms;
    }

    public byte[] SaveToByteArray() => SaveToStream().ToArray();

    // =======================
    // Worksheet
    // =======================
    public IExcelEngine AddWorksheet(string sheetName)
    {
        _activeSheet = Wb.Worksheets.Add(sheetName);
        return this;
    }

    public IExcelEngine DeleteWorksheet(string sheetName)
    {
        Wb.Worksheets.Delete(sheetName);
        return this;
    }

    public IExcelEngine DeleteWorksheets(params string[] sheetNames)
    {
        foreach (var s in sheetNames)
            Wb.Worksheets.Delete(s);
        return this;
    }

    public IExcelEngine CleanAllWorksheets()
    {
        var worksheets = Wb.Worksheets;
        for (int i = worksheets.Count; i > 0; i--)
        {
            worksheets.Delete(i); // 注意索引從 1 開始
        }
        return this;
    }

    public IExcelEngine RenameWorksheet(string oldName, string newName)
    {
        var ws = Wb.Worksheets[oldName];
        if (ws != null) ws.Name = newName;
        return this;
    }

    public IExcelEngine MoveWorksheet(string sheetName, int newIndex)
    {
        if(Wb.TryGetSheet(sheetName, out var ws))
            Wb.Worksheets.MoveAfter(ws.Index, newIndex);
        return this;
    }

    public IExcelEngine CopyWorksheet(string sourceSheetName, string newSheetName)
    {
        //var srcWs = _package.Workbook.Worksheets[sourceSheetName];
        //if (srcWs != null)
        //{
        //    _package.Workbook.Worksheets.Add(newSheetName, srcWs);
        //}
        if (Wb.TryGetSheet(sourceSheetName, out var srcWs))
            Wb.Worksheets.Add(newSheetName, srcWs);
        return this;
    }

    public IExcelEngine HideWorksheet(string sheetName)
    {
        if (Wb.TryGetSheet(sheetName, out var ws))
            ws.Hidden = eWorkSheetHidden.Hidden;
        return this;
    }
    public IExcelEngine VeryHideWorksheet(string sheetName)
    {
        if (Wb.TryGetSheet(sheetName, out var ws))
            ws.Hidden = eWorkSheetHidden.VeryHidden;
        return this;
    }

    public IExcelEngine UnhideWorksheet(string sheetName)
    {
        if (Wb.TryGetSheet(sheetName, out var ws))
            ws.Hidden = eWorkSheetHidden.Visible;
        return this;
    }

    public Dictionary<int, string> GetWorksheetNames()
    {
        var dict = new Dictionary<int, string>();
        int i = 1;
        foreach (var ws in Wb.Worksheets)
            dict.Add(i++, ws.Name);
        return dict;
    }

    public IExcelEngine SetActiveWorksheet(string sheetName)
    {
        _activeSheet = Wb.Worksheets[sheetName];
        return this;
    }

    public string? GetActiveWorksheetName() => _activeSheet?.Name;

    // =======================
    // Row / Column
    // =======================
    public IExcelEngine SetActiveRow(int row) { _activeRow = row; _activeColumn = 1; return this; }
    public int? GetActiveRow() => _activeRow;
    public IExcelEngine SetActiveColumn(int column) { _activeColumn = column; return this; }
    public int? GetActiveColumn() => _activeColumn;
    public IExcelEngine NextRow() { _activeRow++; _activeColumn = 1; return this; }
    public IExcelEngine NextColumn() { _activeColumn++; return this; }

    // =======================
    // Cell 操作
    // =======================
    public IExcelEngine SetCellValue(string sheetName, int row, int column, object value)
    {
        Wb.Worksheets[sheetName].Cells[row, column].Value = value;
        return this;
    }

    public object GetCellValue(string sheetName, int row, int column)
        => Wb.Worksheets[sheetName].Cells[row, column].Value;

    public IExcelEngine MergeCells(string sheetName, int startRow, int startColumn, int endRow, int endColumn)
    {
        Wb.Worksheets[sheetName].Cells[startRow, startColumn, endRow, endColumn].Merge = true;
        return this;
    }

    public IExcelEngine UnmergeCells(string sheetName, int startRow, int startColumn, int endRow, int endColumn)
    {
        var ws = Wb.Worksheets[sheetName];
        ws.Cells[startRow, startColumn, endRow, endColumn].Merge = false;
        return this;
    }

    public IExcelEngine SetCellStyle(string sheetName, int row, int column, DocumentFontStyle style)
    {
        var cell = Wb.Worksheets[sheetName].Cells[row, column];
        if (style.Bold) cell.Style.Font.Bold = true;
        if (!string.IsNullOrEmpty(style.FontColor)) cell.Style.Font.Color.SetColor(System.Drawing.ColorTranslator.FromHtml(style.FontColor));
        if (!string.IsNullOrEmpty(style.BackgroundColor)) cell.Style.Fill.PatternType = ExcelFillStyle.Solid;
        if (!string.IsNullOrEmpty(style.BackgroundColor)) cell.Style.Fill.BackgroundColor.SetColor(System.Drawing.ColorTranslator.FromHtml(style.BackgroundColor));
        if (!string.IsNullOrEmpty(style.HorizontalAlign))
            cell.Style.HorizontalAlignment = style.HorizontalAlign.ToLower() switch
            {
                "center" => ExcelHorizontalAlignment.Center,
                "right" => ExcelHorizontalAlignment.Right,
                _ => ExcelHorizontalAlignment.Left
            };
        return this;
    }

    public IExcelEngine InsertRow(string sheetName, int row)
    {
        Wb.Worksheets[sheetName].InsertRow(row, 1);
        return this;
    }

    public IExcelEngine DeleteRow(string sheetName, int row)
    {
        Wb.Worksheets[sheetName].DeleteRow(row, 1);
        return this;
    }

    public IExcelEngine InsertColumn(string sheetName, int col)
    {
        Wb.Worksheets[sheetName].InsertColumn(col, 1);
        return this;
    }

    public IExcelEngine DeleteColumn(string sheetName, int col)
    {
        Wb.Worksheets[sheetName].DeleteColumn(col, 1);
        return this;
    }

    // =======================
    // Active Sheet / Fluent
    // =======================
    public IExcelEngine SetCellValue(int row, int column, object value) => SetCellValue(ActiveSheet.Name, row, column, value);
    public object GetCellValue(int row, int column) => GetCellValue(ActiveSheet.Name, row, column);
    public IExcelEngine MergeCells(int startRow, int startColumn, int endRow, int endColumn) => MergeCells(ActiveSheet.Name, startRow, startColumn, endRow, endColumn);
    public IExcelEngine UnmergeCells(int startRow, int startColumn, int endRow, int endColumn) => UnmergeCells(ActiveSheet.Name, startRow, startColumn, endRow, endColumn);
    public IExcelEngine SetCellStyle(int row, int column, DocumentFontStyle style) => SetCellStyle(ActiveSheet.Name, row, column, style);
    public IExcelEngine InsertRow(int row) => InsertRow(ActiveSheet.Name, row);
    public IExcelEngine DeleteRow(int row) => DeleteRow(ActiveSheet.Name, row);
    public IExcelEngine InsertColumn(int col) => InsertColumn(ActiveSheet!.Name, col);
    public IExcelEngine DeleteColumn(int col) => DeleteColumn(ActiveSheet.Name, col);

    public IExcelEngine SetCellValue(object value)
    {
        //_package.Workbook.Worksheets[ActiveSheet.Name].Cells[_activeRow, _activeColumn].Value = value;
        ActiveSheet.Cells[_activeRow, _activeColumn].Value = value;
        _activeColumn++;
        return this;
    }

    // =======================
    // Native Object
    // =======================
    public object GetNativeWorkbook() => Wb;
    public object GetNativeWorksheet(string sheetName) => Wb.Worksheets[sheetName];
    public object GetNativeRow(string sheetName, int row) => Wb.Worksheets[sheetName].Cells[row, 1, row, Wb.Worksheets[sheetName].Dimension.End.Column];
    public object GetNativeCell(string sheetName, int row, int column) => Wb.Worksheets[sheetName].Cells[row, column];

}


