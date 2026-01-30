using Veyrin.Scribe.Core.Interfaces;
using Veyrin.Scribe.Core.Models;
namespace Veyrin.Scribe.Core.Contexts;

public class WordContext : IDocumentContext
{
    public readonly IDocEngine _engine;
    public WordContext(IDocEngine engine, DocumentOptions? options = null) => _engine = engine;
    public object GetEngine() => _engine;
    // 常用或通用的方法
    // ==========================================
    // 1. 文件流管理 (快捷方法)
    // ==========================================

    public WordContext Create()
    {
        _engine.CreateDocument();
        return this;
    }

    /// <summary>
    /// 從路徑載入現有文件
    /// </summary>
    public WordContext Open(string filePath)
    {
        _engine.LoadDocument(filePath);
        return this;
    }

    /// <summary>
    /// 儲存並關閉（如果需要資源釋放，可在此擴充）
    /// </summary>
    public void Save(string savePath) => _engine.SaveToFile(savePath);

    /// <summary>
    /// 取得檔案二進制數據，適合 Web API 下載回傳
    /// </summary>
    public byte[] ToBytes() => _engine.SaveToByteArray();

    // ==========================================
    // 2. 內容操作封裝
    // ==========================================

    /// <summary>
    /// 快速填充資料到樣板（取代多個標籤）
    /// </summary>
    public WordContext FillData(Dictionary<string, string> data)
    {
        foreach (var item in data)
        {
            _engine.ReplaceText(item.Key, item.Value);
        }
        return this;
    }

    /// <summary>
    /// 插入一整行文字並自動換行，可指定樣式
    /// </summary>
    public WordContext WriteLine(string text, DocumentFontStyle? style = null)
    {
        if (style != null)
            _engine.AddParagraph(text, style);
        else
            _engine.AddParagraph(text);

        return this;
    }

    /// <summary>
    /// 插入一個簡單的二維陣列作為表格
    /// </summary>
    public WordContext AddSimpleTable(string[,] data)
    {
        int rows = data.GetLength(0);
        int cols = data.GetLength(1);
        _engine.InsertTable(rows, cols);

        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < cols; j++)
            {
                // 注意：這裡假設 tableIndex 是根據目前引擎狀態決定的， 
                // 實作時可能需要 _engine 提供獲取當前 Table Count 的功能
                _engine.SetTableCellValue(-1, i, j, data[i, j]);
            }
        }
        return this;
    }

    // ==========================================
    // 3. 頁面佈局快捷鍵
    // ==========================================

    /// <summary>
    /// 切換到新的一頁並設定為橫向
    /// </summary>
    public WordContext NewLandscapeSection()
    {
        _engine.AddSection()
               .SetPageOrientation(PageOrientation.Landscape);
        return this;
    }
}