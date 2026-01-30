using Veyrin.Scribe.Core.Interfaces;

namespace Veyrin.Scribe.Pdf.PdfSharp;

public class PdfEngine : IPdfEngine, IEngine
{
    private readonly MemoryStream _ms;
    //private readonly PdfDocument _pdf;
    //private readonly Document _document;

    public PdfEngine()
    {
        _ms = new MemoryStream();
        //    _pdf = new PdfDocument(new PdfWriter(_ms));
        //    _document = new Document(_pdf);
    }

    //public object AddParagraph(string text)
    //{
    //    _document.Add(new Paragraph(text));
    //    return _document;
    //}

    //public object AddTable<T>(IEnumerable<T> data)
    //{
    //    // TODO: 根據 T 自動生成表格
    //    return _document;
    //}

    //public object AddWorksheet(string name) => null;
    //public object SetCellValue(string sheetName, int row, int column, object value) => null;
    //public object MergeCells(string sheetName, int startRow, int startColumn, int endRow, int endColumn) => null;
    //public object UnmergeCells(string sheetName, int startRow, int startColumn, int endRow, int endColumn) => null;
    //public object SetCellStyle(string sheetName, int row, int column, DocumentFontStyle style) => null;

    //public object GetNativeDocument() => _document;

    //public void SaveToFile(string path)
    //{
    //    _document.Close();
    //    File.WriteAllBytes(path, _ms.ToArray());
    //}

    public MemoryStream SaveToStream()
    {
        //_document.Close();
        //_ms.Position = 0;
        //return _ms;
        throw new NotImplementedException();
    }

    public byte[] SaveToByteArray()
    {
        throw new NotImplementedException();
        //_document.Close();
        //return _ms.ToArray();
    }

}