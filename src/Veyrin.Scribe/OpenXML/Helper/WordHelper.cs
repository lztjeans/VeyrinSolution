using DocumentFormat.OpenXml.Wordprocessing;
using HorizontalAlignment = Veyrin.Scribe.Core.Models.HorizontalAlignment;


namespace Veyrin.Scribe.OpenXML.Helper;

public static class WordHelper
{
    public static JustificationValues MapAlignment(HorizontalAlignment alignment) => alignment switch
    {
        HorizontalAlignment.Center => JustificationValues.Center,
        HorizontalAlignment.Right => JustificationValues.Right,
        _ => JustificationValues.Left
    };
}
