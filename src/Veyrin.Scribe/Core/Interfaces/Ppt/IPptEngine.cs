using Veyrin.Scribe.Core.Interfaces.Word;
using Veyrin.Scribe.Core.Models;

/// <summary>
/// 定義 PowerPoint 操作引擎的共用介面。
/// </summary>
public interface IPptEngine
{
    // =============================
    //  Presentation 基本操作
    // =============================
    IPptEngine CreatePresentation();
    IPptEngine LoadPresentation(string path);
    IPptEngine SaveToFile(string path);
    // =============================
    //  Slide (投影片) 操作
    // =============================
    /// <summary>新增投影片，需指定版面配置。</summary>
    IPptEngine AddSlide(SlideLayoutType layout);
    IPptEngine DeleteSlide(int slideIndex);
    IPptEngine MoveSlide(int oldIndex, int newIndex);
    /// <summary>設定目前作用中的投影片。</summary>
    IPptEngine SetActiveSlide(int slideIndex);
    int GetSlideCount();

    // =============================
    //  Shape & Text (物件與文字)
    // =============================
    /// <summary>向目前投影片加入文字方塊。</summary>
    IPptEngine AddTextBox(float x, float y, float width, float height, string text);
    /// <summary>向目前投影片加入文字方塊並設定樣式。</summary>
    IPptEngine AddTextBox(float x, float y, float width, float height, string text, DocumentFontStyle style);
    /// <summary>取代投影片中的預位符 (Placeholder) 文字。</summary>
    IPptEngine ReplacePlaceholderText(string placeholderTag, string text);

    // =============================
    //  多媒體與圖表
    // =============================
    IWordEngine InsertImage(float x, float y, float width, float height, string imagePath);
    IWordEngine InsertTable(float x, float y, float width, float height, int rows, int cols);
    // PPT 常見需求：自動產生圖表
    IWordEngine InsertChart(ChartType type, float x, float y, float width, float height);

    // =============================
    //  投影片備註與動畫
    // =============================
    IPptEngine SetSlideNotes(string notes);
    /// <summary>設定投影片切換動畫。</summary>
    IPptEngine SetTransition(TransitionEffect effect);

    // =============================
    //  原生物件存取
    // =============================
    object GetNativePresentation();
    object GetNativeSlide(int index);
    object GetNativeShape(int slideIndex, int shapeIndex);
}