using System.ComponentModel;

namespace Veyrin.Scribe.Core.Models;

/// <summary>
/// 頁面方向（常用於 Word 或是 Excel 列印設定）
/// </summary>
public enum PageOrientation
{
    Portrait,  // 直向
    Landscape  // 橫向
}

/// <summary>
/// 對齊方式(水平)
/// </summary>
public enum HorizontalAlignment
{
    Left,
    Center,
    Right,
    Justify // 分散對齊
}

/// <summary>
/// 對齊方式(垂直)
/// </summary>
public enum VerticalAlignment
{
    Top,
    Center,
    Bottom,
}


/// <summary>
/// 投影片版面配置類型（對應 PowerPoint 內建的版面）
/// </summary>
public enum SlideLayoutType
{
    Title,             // 標題投影片
    Text,              // 標題與文字
    TwoColumnText,     // 兩欄文字
    Blank,             // 空白
    TitleOnly,         // 僅標題
    MediaAndText,      // 媒體與文字
    ChartAndText       // 圖表與文字
}

/// <summary>
/// 圖表類型
/// </summary>
public enum ChartType
{
    Bar,      // 條形圖
    Column,   // 柱狀圖
    Line,     // 折線圖
    Pie,      // 圓餅圖
    Scatter,  // 散佈圖
    Area      // 面積圖
}

/// <summary>
/// 投影片切換效果
/// </summary>
public enum TransitionEffect
{
    None,
    Fade,     // 淡出
    Push,     // 推入
    Wipe,     // 擦去
    Split,    // 分割
    Morph     // 轉化 (新版 PPT 常用)
}

public enum EngineNames
{
    [Description("")]
    NONE,
    [Description("xXML")]
    ClosedXML,
    [Description("ep+")]
    EPPLUS,
    [Description("nxls")]
    NPOIXLS,
    [Description("ndoc")]
    NPOIDOC,
    [Description("nppt")]
    NPOIPPT,
    [Description("oxls")]
    OPENXLS,
    [Description("odoc")]
    OPENDOC,
    [Description("oppt")]
    OPENPPT,
    [Description("ccssvv")]
    CSV,
    [Description("xceed")]
    DOCX,
    [Description("pdf#")]
    PDF,
    [Description("itxt7")]
    TEXT7

}

public enum ExcelLineStyle
{
    None,
    Thin,       // 細線
    Medium,     // 中粗線
    Thick,      // 粗線
    Dashed,     // 虛線
    Dotted,     // 點線
    Double      // 雙線
}

[Flags]
public enum BorderSide
{
    None = 0,
    Top = 1,
    Bottom = 2,
    Left = 4,
    Right = 8,
    All = Top | Bottom | Left | Right
}

public class BorderSettings
{
    public ExcelLineStyle LineStyle { get; set; } = ExcelLineStyle.Thin;
    public System.Drawing.Color Color { get; set; } = System.Drawing.Color.Black;
    public BorderSide Side { get; set; } = BorderSide.All;
}