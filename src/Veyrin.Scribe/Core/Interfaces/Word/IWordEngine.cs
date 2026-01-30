using Veyrin.Scribe.Core.Models;

namespace Veyrin.Scribe.Core.Interfaces.Word;


/// <summary>
/// 定義 Word 操作引擎的共用介面。
/// </summary>
public interface IWordEngine
{
    // =============================
    //  Document 基本操作
    // =============================
    IWordEngine CreateDocument();
    IWordEngine LoadDocument(string path);
    IWordEngine SaveToFile(string path);
    // =============================
    /// <summary>章節操作</summary>
    IWordEngine AddSection();
    /// <summar>增加頁面</summary>
    IWordEngine AddPageBreak();
    /// <summar>頁面操作</summary>
    IWordEngine SetPageOrientation(PageOrientation orientation); // 橫向或縱向

    // =============================
    //  內容寫入 (Paragraph)
    // =============================
    /// <summary>新增段落並設定文字。</summary>
    IWordEngine AddParagraph(string text);
    /// <summary>新增段落並套用樣式。</summary>
    IWordEngine AddParagraph(string text, DocumentFontStyle style);
    /// <summary>在目前段落追加文字 (不換行)。</summary>
    IWordEngine AppendText(string text);
    /// <summary>設定目前段落對齊方式。</summary>
    IWordEngine SetAlignment(HorizontalAlignment alignment);

    // =============================
    //  表格操作 (Table)
    // =============================
    /// <summary>在目前位置插入表格。</summary>
    IWordEngine InsertTable(int rows, int columns);
    /// <summary>設定表格儲存格內容。</summary>
    IWordEngine SetTableCellValue(int tableIndex, int row, int col, object value);
    /// <summary>合併表格儲存格。</summary>
    IWordEngine MergeTableCells(int tableIndex, int fromRow, int fromCol, int toRow, int toCol);

    // =============================
    /// <summary>圖片</summary>
    IWordEngine InsertImage(string imagePath, float width, float height);
    /// <summary>頁首</summary>
    IWordEngine SetHeader(string text);
    /// <summary>頁尾</summary>
    IWordEngine SetFooter(string text);

    // =============================
    //  取代與搜尋
    // =============================
    /// <summary>全域取代關鍵字 (常用於樣板填充)。</summary>
    IWordEngine ReplaceText(string oldValue, string newValue);

    // =============================
    //  原生物件存取
    // =============================
    object GetNativeDocument();
    object GetNativeParagraph(int index);
    object GetNativeTable(int index);
}