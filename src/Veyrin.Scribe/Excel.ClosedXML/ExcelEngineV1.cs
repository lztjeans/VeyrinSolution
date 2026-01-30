using ClosedXML.Excel;
using System.Data;
using Veyrin.Scribe.Core.Interfaces;
using Veyrin.Scribe.Core.Interfaces.Excel;
using Veyrin.Scribe.Core.Models;
using Veyrin.Scribe.Excel.ClosedXML.Helper;
namespace Veyrin.Scribe.Excel.ClosedXML;
public class ExcelEngineV1 : IExcelEngine, IEngine
{
    private XLWorkbook? _workbook;
    private string? _activeSheetName;
    private int? _activeRow;
    private int? _activeColumn;
    // =======================
    private XLWorkbook Workbook => _workbook.EnsureWorkbook();
    private string ActiveSheetName => _activeSheetName.EnsureActiveSheet();
    // =======================
    // Workbook 基本操作
    // =======================
    public IExcelEngine CreateXLSWorkbook() => throw new NotSupportedException("ClosedXML 僅支援 .xlsx 格式。");
    public IExcelEngine CreateXLSXWorkbook()
    {
        _workbook = new XLWorkbook();
        return this;
    }
    public IExcelEngine LoadWorkbook(string path)
    {
        _workbook = new XLWorkbook(path);
        return this;
    }
    public IExcelEngine SaveToFile(string path)
    {
        Workbook.SaveAs(path);
        return this;
    }
    public MemoryStream SaveToStream()
    {
        var ms = new MemoryStream();
        Workbook.SaveAs(ms);
        ms.Position = 0;
        return ms;
    }
    public byte[] SaveToByteArray()
    {
        using var ms = SaveToStream();
        return ms.ToArray();
    }
    // =======================
    // Worksheet 操作
    // =======================
    public IExcelEngine AddWorksheet(string sheetName)
    {
        Workbook.Worksheets.Add(sheetName);
        _activeSheetName = sheetName;
        return this;
    }
    public IExcelEngine DeleteWorksheet(string sheetName)
    {
        Workbook.Worksheet(sheetName).Delete();
        return this;
    }
    public IExcelEngine DeleteWorksheets(params string[] sheetNames)
    {
        //var _w = Workbook;
        foreach (var name in sheetNames)
            Workbook.Worksheet(name).Delete();
        return this;
    }
    public IExcelEngine CleanAllWorksheets()
    {
        foreach (var ws in Workbook.Worksheets)
            ws.Delete();
        return this;
    }
    public IExcelEngine RenameWorksheet(string oldName, string newName)
    {
        Workbook.Worksheet(oldName).Name = newName;
        return this;
    }
    public IExcelEngine MoveWorksheet(string sheetName, int newIndex)
    {
        Workbook.Worksheet(sheetName).Position = newIndex;
        return this;
    }
    public IExcelEngine CopyWorksheet(string sourceSheetName, string newSheetName)
    {
        Workbook.Worksheet(sourceSheetName).CopyTo(newSheetName);
        return this;
    }
    public IExcelEngine HideWorksheet(string sheetName)
    {
        Workbook.Worksheet(sheetName).Hide();
        return this;
    }
    public IExcelEngine VeryHideWorksheet(string sheetName) => throw new NotSupportedException("ClosedXML 不支援");
    public IExcelEngine UnhideWorksheet(string sheetName)
    {
        Workbook.Worksheet(sheetName).Unhide();
        return this;
    }
    public Dictionary<int, string> GetWorksheetNames()
    {
        var dict = new Dictionary<int, string>();
        int i = 1;
        var _w = Workbook;
        foreach (var ws in _w.Worksheets)
            dict.Add(i++, ws.Name);
        return dict;
    }
    public IExcelEngine SetActiveWorksheet(string sheetName)
    {
        _activeSheetName = sheetName;
        return this;
    }
    public string? GetActiveWorksheetName() => ActiveSheetName;
    // =======================
    // Row / Column 狀態
    // =======================
    public IExcelEngine SetActiveRow(int row) { _activeRow = row; _activeColumn = 1; return this; }
    public int? GetActiveRow() => _activeRow;
    public IExcelEngine SetActiveColumn(int column) { _activeColumn = column; return this; }
    public int? GetActiveColumn() => _activeColumn;
    public IExcelEngine NextRow() { if (_activeRow == null) throw new InvalidOperationException("Active row not set."); _activeRow++; _activeColumn = 1; return this; }
    public IExcelEngine NextColumn() { if (_activeColumn == null) throw new InvalidOperationException("Active column not set."); _activeColumn++; return this; }
    // =======================
    // Cell 操作 (指定 sheet)
    // =======================
    public IExcelEngine SetCellValue(string sheetName, int row, int column, object value)
    {
        var ws = Workbook.Worksheet(sheetName);
        var cell = ws.Cell(row, column);
        //XLCellValue _value = (XLCellValue)value;
        //cell.SetValue(_value);
        if (value == null)
        {
            cell.SetValue(string.Empty);
        }
        else if (value is DateTime dt)
        {
            cell.SetValue(dt);
        }
        else if (value is bool b)
        {
            cell.SetValue(b);
        }
        else if (value is int i)
        {
            cell.SetValue(i);
        }
        else if (value is double d)
        {
            cell.SetValue(d);
        }
        else if (value is decimal dec)
        {
            cell.SetValue(Convert.ToDouble(dec));
        }
        else
        {
            // 預設用文字
            cell.SetValue(value.ToString());
        }
        return this;
    }
    public object GetCellValue(string sheetName, int row, int column)
        => Workbook.Worksheet(sheetName).Cell(row, column).Value;
    public IExcelEngine MergeCells(string sheetName, int startRow, int startColumn, int endRow, int endColumn)
    {
        Workbook.Worksheet(sheetName).Range(startRow, startColumn, endRow, endColumn).Merge();
        return this;
    }
    public IExcelEngine UnmergeCells(string sheetName, int startRow, int startColumn, int endRow, int endColumn)
    {
        Workbook.Worksheet(sheetName).Range(startRow, startColumn, endRow, endColumn).Unmerge();
        return this;
    }
    public IExcelEngine SetCellStyle(string sheetName, int row, int column, DocumentFontStyle style)
    {
        var cell = Workbook.Worksheet(sheetName).Cell(row, column);
        cell.Style.Font.Bold = style.Bold;
        if (StringUtils.IsNotEmpty(style.FontColor)) cell.Style.Font.FontColor = XLColor.FromHtml(style.FontColor);
        if (StringUtils.IsNotEmpty(style.BackgroundColor)) cell.Style.Fill.BackgroundColor = XLColor.FromHtml(style.BackgroundColor);
        cell.Style.Alignment.Horizontal = style.HorizontalAlign.Convert();
        cell.Style.Alignment.Vertical = style.VerticalAlign.Convert();
        return this;
    }
    public IExcelEngine InsertRow(string sheetName, int row)
    {
        Workbook.Worksheet(sheetName).Row(row).InsertRowsAbove(1);
        return this;
    }
    public IExcelEngine DeleteRow(string sheetName, int row)
    {
        Workbook.Worksheet(sheetName).Row(row).Delete();
        return this;
    }
    public IExcelEngine InsertColumn(string sheetName, int column)
    {
        Workbook.Worksheet(sheetName).Column(column).InsertColumnsBefore(1);
        return this;
    }
    public IExcelEngine DeleteColumn(string sheetName, int column)
    {
        Workbook.Worksheet(sheetName).Column(column).Delete();
        return this;
    }
    // =======================
    // Cell 操作 (Active Sheet)
    // =======================
    public IExcelEngine SetCellValue(int row, int column, object value)
    {
        return SetCellValue(ActiveSheetName, row, column, value);
    }
    public object GetCellValue(int row, int column)
    {
        return GetCellValue(ActiveSheetName, row, column);
    }
    public IExcelEngine MergeCells(int startRow, int startColumn, int endRow, int endColumn)
    {
        return MergeCells(ActiveSheetName, startRow, startColumn, endRow, endColumn);
    }
    public IExcelEngine UnmergeCells(int startRow, int startColumn, int endRow, int endColumn)
    {
        return UnmergeCells(ActiveSheetName, startRow, startColumn, endRow, endColumn);
    }
    public IExcelEngine SetCellStyle(int row, int column, DocumentFontStyle style)
    {
        return SetCellStyle(ActiveSheetName, row, column, style);
    }
    public IExcelEngine InsertRow(int row)
    {
        return InsertRow(ActiveSheetName, row);
    }
    public IExcelEngine DeleteRow(int row)
    {
        return DeleteRow(ActiveSheetName, row);
    }
    public IExcelEngine InsertColumn(int column)
    {
        return InsertColumn(ActiveSheetName, column);
    }
    public IExcelEngine DeleteColumn(int column)
    {
        return DeleteColumn(ActiveSheetName, column);
    }
    // =======================
    // Cell 操作 (Active Row/Col)
    // =======================
    public IExcelEngine SetCellValue(object value)
    {
        if (_activeRow == null) throw new InvalidOperationException("Active row not set.");
        _activeColumn ??= 1;
        SetCellValue(ActiveSheetName, _activeRow.Value, _activeColumn.Value, value);
        _activeColumn++;
        return this;
    }
    // =======================
    // Native Object
    // =======================
    public object GetNativeWorkbook() => Workbook;
    public object GetNativeWorksheet(string sheetName) => Workbook.Worksheet(sheetName);
    public object GetNativeRow(string sheetName, int row) => Workbook.Worksheet(sheetName).Row(row);
    public object GetNativeCell(string sheetName, int row, int column) => Workbook.Worksheet(sheetName).Cell(row, column);
    public IExcelEngine ImportDataTable(string sheetName, int startRow, int startColumn, DataTable dt, bool printHeaders = true)
    {
        throw new NotImplementedException();
    }
    public IExcelEngine AutoFitColumn(string sheetName, int column)
    {
        throw new NotImplementedException();
    }
    public IExcelEngine AutoFitColumns(string sheetName, int startColumn, int endColumn)
    {
        throw new NotImplementedException();
    }
    public int GetLastRowIndex(string sheetName)
    {
        throw new NotImplementedException();
    }
    public IExcelEngine SetRangeStyle(string sheetName, int startRow, int startColumn, int endRow, int endColumn, DocumentFontStyle style)
    {
        throw new NotImplementedException();
    }
    public IExcelEngine SetColumnWidth(string sheetName, int column, double width)
    {
        throw new NotImplementedException();
    }
    public IExcelEngine SetRowHeight(string sheetName, int row, double height)
    {
        throw new NotImplementedException();
    }
    public IExcelEngine SetCellBorder(int row, int column, BorderSettings settings)
    {
        throw new NotImplementedException();
    }
    public IExcelEngine SetRangeBorder(int startRow, int startColumn, int endRow, int endColumn, BorderSettings settings, bool insideLines = false)
    {
        throw new NotImplementedException();
    }
    public IExcelEngine SetTableBorder(int startRow, int startColumn, int endRow, int endColumn)
    {
        throw new NotImplementedException();
    }
    public IExcelEngine SetCellFormula(string sheetName, int row, int column, string formula)
    {
        throw new NotImplementedException();
    }
    public IExcelEngine FreezePanes(string sheetName, int row, int column)
    {
        throw new NotImplementedException();
    }
    public IExcelEngine SetAutoFilter(string sheetName, int startRow, int startColumn, int endRow, int endColumn)
    {
        throw new NotImplementedException();
    }
    public IExcelEngine InsertImage(string sheetName, byte[] imageBytes, int row, int column, int width, int height)
    {
        throw new NotImplementedException();
    }
    public Tuple<int, int>? FindCellValue(string sheetName, string searchText)
    {
        throw new NotImplementedException();
    }

    public IExcelEngine SetCellFormula(int row, int column, string formula)
    => SetCellFormula(ActiveSheetName, row, column, formula);

    public IExcelEngine FreezePanes(int row, int column) => FreezePanes(ActiveSheetName, row, column);

    public IExcelEngine SetAutoFilter(int startRow, int startColumn, int endRow, int endColumn)
        => SetAutoFilter(ActiveSheetName, startRow, startColumn, endRow, endColumn);

    public IExcelEngine InsertImage(byte[] imageBytes, int row, int column, int width, int height)
        => InsertImage(ActiveSheetName, imageBytes, row, column, width, height);

    public Tuple<int, int>? FindCellValue(string searchText) => FindCellValue(searchText);

    public IExcelEngine AutoFitColumn(int column) => AutoFitColumn(ActiveSheetName, column);

    public IExcelEngine AutoFitColumns(int startColumn, int endColumn) => AutoFitColumns(ActiveSheetName, startColumn, endColumn);

    public int GetLastRowIndex() => GetLastRowIndex(ActiveSheetName);

    public IExcelEngine SetRangeStyle(int startRow, int startColumn, int endRow, int endColumn, DocumentFontStyle style)
    => SetRangeStyle(ActiveSheetName, startRow, startColumn, endRow, endColumn, style);

    public IExcelEngine SetColumnWidth(int column, double width) => SetColumnWidth(ActiveSheetName, column, width);

    public IExcelEngine SetRowHeight(int row, double height) => SetRowHeight(ActiveSheetName, row, height);
}