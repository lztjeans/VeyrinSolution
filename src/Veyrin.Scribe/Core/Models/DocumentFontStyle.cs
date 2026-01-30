namespace Veyrin.Scribe.Core.Models;

public class DocumentFontStyle
{
    public string? FontName { get; set; }
    public double? FontSize { get; set; }
    public bool Bold { get; set; } = false;
    public bool Italic { get; set; } = false;      // 新增：斜體
    public bool Underline { get; set; } = false;   // 新增：底線

    /// <summary> 建議格式：Hex 色碼 (如 #FFFFFF) 或顏色名稱 </summary>
    public string? FontColor { get; set; }
    public string? BackgroundColor { get; set; }

    /// <summary> left, center, right, justify </summary>
    public HorizontalAlignment HorizontalAlign { get; set; } = HorizontalAlignment.Left;

    /// <summary> top, center, bottom </summary>
    public VerticalAlignment VerticalAlign { get; set; } = VerticalAlignment.Center;

    // 範例：將 Hex 字串轉為 System.Drawing.Color
    private static System.Drawing.Color ColorFromHex(string? hex)
    {
        if (StringUtils.IsEmpty(hex)) return System.Drawing.Color.Black;
        try
        {
            return System.Drawing.ColorTranslator.FromHtml(hex);
        }
        catch
        {
            return System.Drawing.Color.Black; // 預設值
        }
    }
}