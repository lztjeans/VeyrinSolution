using Veyrin.Scribe.Core.Interfaces;
using Veyrin.Scribe.Core.Interfaces.Word;
using Veyrin.Scribe.Core.Models;

namespace Veyrin.Scribe.NPOI;

public class WordEngine : IWordEngine, IEngine
{
    public IWordEngine AddPageBreak()
    {
        throw new NotImplementedException();
    }

    public IWordEngine AddParagraph(string text)
    {
        throw new NotImplementedException();
    }

    public IWordEngine AddParagraph(string text, DocumentFontStyle style)
    {
        throw new NotImplementedException();
    }

    public IWordEngine AddSection()
    {
        throw new NotImplementedException();
    }

    public IWordEngine AppendText(string text)
    {
        throw new NotImplementedException();
    }

    public IWordEngine CreateDocument()
    {
        throw new NotImplementedException();
    }

    public object GetNativeDocument()
    {
        throw new NotImplementedException();
    }

    public object GetNativeParagraph(int index)
    {
        throw new NotImplementedException();
    }

    public object GetNativeTable(int index)
    {
        throw new NotImplementedException();
    }

    public IWordEngine InsertImage(string imagePath, float width, float height)
    {
        throw new NotImplementedException();
    }

    public IWordEngine InsertTable(int rows, int columns)
    {
        throw new NotImplementedException();
    }

    public IWordEngine LoadDocument(string path)
    {
        throw new NotImplementedException();
    }

    public IWordEngine MergeTableCells(int tableIndex, int fromRow, int fromCol, int toRow, int toCol)
    {
        throw new NotImplementedException();
    }

    public IWordEngine ReplaceText(string oldValue, string newValue)
    {
        throw new NotImplementedException();
    }

    public byte[] SaveToByteArray()
    {
        throw new NotImplementedException();
    }

    public IWordEngine SaveToFile(string path)
    {
        throw new NotImplementedException();
    }

    public MemoryStream SaveToStream()
    {
        throw new NotImplementedException();
    }

    public IWordEngine SetAlignment(HorizontalAlignment alignment)
    {
        throw new NotImplementedException();
    }

    public IWordEngine SetFooter(string text)
    {
        throw new NotImplementedException();
    }

    public IWordEngine SetHeader(string text)
    {
        throw new NotImplementedException();
    }

    public IWordEngine SetPageOrientation(PageOrientation orientation)
    {
        throw new NotImplementedException();
    }

    public IWordEngine SetTableCellValue(int tableIndex, int row, int col, object value)
    {
        throw new NotImplementedException();
    }
}