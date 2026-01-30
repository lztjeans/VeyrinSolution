using Veyrin.Scribe.Core.Models;
namespace Veyrin.Scribe.Core.Contexts;

public class ExcelContext : IDocumentContext
{
    public readonly IExcelEngine _engine;

    public ExcelContext(IExcelEngine engine, DocumentOptions? options = null) => _engine = engine;

    public void CreateXLSWorkbook() => _engine.CreateXLSWorkbook();
    public void CreateXLSXWorkbook() => _engine.CreateXLSXWorkbook();
    public void LoadWorkbook(string path) => _engine.LoadWorkbook(path);
    public void SaveToFile(string path) => _engine.SaveToFile(path);
    public MemoryStream SaveToStream() => _engine.SaveToStream();
    public byte[] SaveToByteArray() => _engine.SaveToByteArray();

    public void AddWorksheet(string sheetName) => _engine.AddWorksheet(sheetName);
    public void SetCellValue(string sheetName, int row, int column, object value) =>
        _engine.SetCellValue(sheetName, row, column, value);
    public object? GetCellValue(string sheetName, int row, int column) =>
        _engine.GetCellValue(sheetName, row, column);

    public void MergeCells(string sheetName, int startRow, int startColumn, int endRow, int endColumn) =>
        _engine.MergeCells(sheetName, startRow, startColumn, endRow, endColumn);

    public void UnmergeCells(string sheetName, int startRow, int startColumn, int endRow, int endColumn) =>
        _engine.UnmergeCells(sheetName, startRow, startColumn, endRow, endColumn);

    public void SetCellStyle(string sheetName, int row, int column, DocumentFontStyle style) =>
        _engine.SetCellStyle(sheetName, row, column, style);

    public void InsertRow(string sheetName, int row) => _engine.InsertRow(sheetName, row);
    public void DeleteRow(string sheetName, int row) => _engine.DeleteRow(sheetName, row);
    public void InsertColumn(string sheetName, int column) => _engine.InsertColumn(sheetName, column);
    public void DeleteColumn(string sheetName, int column) => _engine.DeleteColumn(sheetName, column);
    public object GetEngine() => _engine;



}
//public void CreateFile() => throw new NotImplementedException();

//public void LoadFile(string path) => throw new NotImplementedException();

//public byte[] SaveToBytes() => throw new NotImplementedException();

//public string ReadText() => throw new NotImplementedException();

//public T Read<T>() where T : class => throw new NotImplementedException();

//public void WriteText(string content) => throw new NotImplementedException();

//public void Write<T>(T content) where T : class => throw new NotImplementedException();
//public object AddParagraph(string text) => throw new NotImplementedException();

//public object AddTable<T>(IEnumerable<T> data) => throw new NotImplementedException();

//public object GetNativeDocument() => throw new NotImplementedException();

