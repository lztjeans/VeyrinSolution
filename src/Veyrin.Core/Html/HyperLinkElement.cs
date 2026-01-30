namespace Veyrin.Core.Html;
public class HyperLinkElement : AbstractElements
{
    public string BindEvent { get; set; } = string.Empty;

    public static HyperLinkElement Create(string id, string label, string eventNm)
    {
        string actId = $"hl_{id}";
        return new HyperLinkElement
        {
            Id = $"hl_{id}",
            Label = label,
            IsRequired = false,
            BindEvent = $"{eventNm}('{actId}')"
        };
    }
}