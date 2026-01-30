using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using Veyrin.Scribe.Core.Models;

public class XlxStyleManager
{
    private readonly Stylesheet _stylesheet;
    private readonly Dictionary<string, uint> _fontCache = new();
    private readonly Dictionary<string, uint> _fillCache = new();
    private readonly Dictionary<string, uint> _borderCache = new();
    private readonly Dictionary<string, uint> _cellFormatCache = new();

    public XlxStyleManager(WorkbookStylesPart stylesPart)
    {
        // 初始化 Stylesheet 結構
        if (stylesPart.Stylesheet == null)
        {
            stylesPart.Stylesheet = new Stylesheet(
                new Fonts(new Font()),       // 預設索引 0
                new Fills(new Fill(new PatternFill { PatternType = PatternValues.None })), // 預設索引 0
                new Borders(new Border()),   // 預設索引 0
                new CellFormats(new CellFormat()) // 預設索引 0
            );
        }
        _stylesheet = stylesPart.Stylesheet;
    }

    public uint GetStyleIndex(DocumentFontStyle style)
    {
        // 1. 處理字體 (Font)
        string fontKey = $"{style.FontName}-{style.FontSize}-{style.FontColor}-{style.Bold}";
        if (!_fontCache.TryGetValue(fontKey, out uint fontId))
        {
            var font = new Font();
            if (style.Bold) font.Append(new Bold());
            font.Append(new FontSize { Val = style.FontSize });
            font.Append(new Color { Rgb = style.FontColor }); // 例如 "FF0000"
            font.Append(new FontName { Val = style.FontName });

            _stylesheet.Fonts!.Append(font);
            fontId = (uint)(_stylesheet.Fonts!.Count!++ - 1);
            _fontCache[fontKey] = fontId;
        }

        // 2. 處理填滿 (Fill)
        string fillKey = style.BackgroundColor ?? "Transparent";
        if (!_fillCache.TryGetValue(fillKey, out uint fillId))
        {
            if (style.BackgroundColor == null)
            {
                fillId = 0; // 預設透明
            }
            else
            {
                var fill = new Fill(new PatternFill(new ForegroundColor { Rgb = style.BackgroundColor })
                { PatternType = PatternValues.Solid });
                _stylesheet.Fills!.Append(fill);
                fillId = (uint)(_stylesheet.Fills.Count!++ - 1);
            }
            _fillCache[fillKey] = fillId;
        }

        // 3. 組合 CellFormat
        string formatKey = $"f{fontId}-b{fillId}";
        if (!_cellFormatCache.TryGetValue(formatKey, out uint formatId))
        {
            var cellFormat = new CellFormat
            {
                FontId = fontId,
                FillId = fillId,
                ApplyFont = true,
                ApplyFill = fillId > 0
            };
            _stylesheet.CellFormats!.Append(cellFormat);
            formatId = (uint)(_stylesheet.CellFormats.Count!++ - 1);
            _cellFormatCache[formatKey] = formatId;
        }

        return formatId;
    }
}
