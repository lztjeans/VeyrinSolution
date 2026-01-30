using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using Veyrin.Scribe.Core.Interfaces;
using Veyrin.Scribe.Core.Interfaces.Word;
using Veyrin.Scribe.Core.Models;
using Veyrin.Scribe.OpenXML.Helper;
using HorizontalAlignment = Veyrin.Scribe.Core.Models.HorizontalAlignment;

namespace Veyrin.Scribe.OpenXML.Engine;
public class WordEngine : IWordEngine, IEngine
{
    private WordprocessingDocument? _document;
    private MainDocumentPart? _mainPart;
    private Body? _body;
    // 用於追蹤當前操作的狀態
    private HorizontalAlignment _currentAlignment = HorizontalAlignment.Left;
    public IWordEngine CreateDocument()
    {
        var stream = new MemoryStream();
        _document = WordprocessingDocument.Create(stream, WordprocessingDocumentType.Document);
        _mainPart = _document.AddMainDocumentPart();
        _mainPart.Document = new Document(new Body());
        _body = _mainPart.Document.Body;
        return this;
    }
    public IWordEngine LoadDocument(string path)
    {
        // 為了支援 SaveToStream，建議載入到 MemoryStream
        var byteArray = File.ReadAllBytes(path);
        var memStream = new MemoryStream();
        memStream.Write(byteArray, 0, byteArray.Length);

        _document = WordprocessingDocument.Open(memStream, true);
        _mainPart = _document.MainDocumentPart;
        _body = _mainPart?.Document.Body;
        return this;
    }
    public IWordEngine AddParagraph(string text)
    {
        if (_body == null) throw new InvalidOperationException("文件尚未初始化");

        var para = new Paragraph();

        // 處理對齊
        var pPr = new ParagraphProperties();
        pPr.AppendChild(new Justification { Val = WordHelper.MapAlignment(_currentAlignment) });
        para.AppendChild(pPr);

        var run = para.AppendChild(new Run());
        run.AppendChild(new Text(text));

        _body.AppendChild(para);
        return this;
    }

    public IWordEngine AppendText(string text)
    {
        // 取得最後一個段落，若無則新增
        var lastPara = _body?.Elements<Paragraph>().LastOrDefault() ?? (Paragraph)_body!.AppendChild(new Paragraph());
        var run = lastPara.AppendChild(new Run());
        run.AppendChild(new Text(text));
        return this;
    }

    public IWordEngine InsertTable(int rows, int columns)
    {
        Table table = new();

        // 設定表格樣式 (邊框)
        TableProperties tblPr = new(
            new TableBorders(
                new TopBorder { Val = BorderValues.Single, Size = 4 },
                new BottomBorder { Val = BorderValues.Single, Size = 4 },
                new LeftBorder { Val = BorderValues.Single, Size = 4 },
                new RightBorder { Val = BorderValues.Single, Size = 4 },
                new InsideHorizontalBorder { Val = BorderValues.Single, Size = 4 },
                new InsideVerticalBorder { Val = BorderValues.Single, Size = 4 }
            )
        );
        table.AppendChild(tblPr);

        for (int i = 0; i < rows; i++)
        {
            TableRow tr = new();
            for (int j = 0; j < columns; j++)
            {
                TableCell tc = new(new Paragraph(new Run(new Text(""))));
                tr.Append(tc);
            }
            table.Append(tr);
        }

        _body?.AppendChild(table);
        return this;
    }

    public IWordEngine ReplaceText(string oldValue, string newValue)
    {
        foreach (var text in _body!.Descendants<Text>())
        {
            if (text.Text.Contains(oldValue))
            {
                text.Text = text.Text.Replace(oldValue, newValue);
            }
        }
        return this;
    }

    public IWordEngine AddPageBreak()
    {
        _body?.AppendChild(new Paragraph(new Run(new Break { Type = BreakValues.Page })));
        return this;
    }

    public byte[] SaveToByteArray()
    {
        _document?.Dispose(); // 必須先釋放以確保內容寫入 Stream
        if (_document?.PackageProperties is null) return [];
        // 這裡需要重新取得載入時的 Stream
        return []; // 建議實作時紀錄原始 Stream
    }

    public IWordEngine SaveToFile(string path)
    {
        _document?.Clone(path).Dispose();
        return this;
    }

    public object GetNativeDocument() => _document!;






    public IWordEngine AddParagraph(string text, DocumentFontStyle style)
    {
        throw new NotImplementedException();
    }

    public IWordEngine AddSection()
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



    public IWordEngine MergeTableCells(int tableIndex, int fromRow, int fromCol, int toRow, int toCol)
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