namespace Veyrin.Core.Html;
public class ButtonElement : AbstractElements
{
    //public string BindEvent { get; set; } = string.Empty;
    public static ButtonElement Create(string id, string label, string eventNm, string css = "btn btn-primary btn-sm me-2", string style = "")
    {
        return new ButtonElement
        {
            Id = $"btn_{id}",
            Label = label,
            //BindEvent = eventNm,
            IsRequired = false,
            Attributes = new Dictionary<string, string>() {
                { "class", css },
                { "style", style },
                { "onclick", eventNm },
                //{ "maxlength", "" },
                //{ "readonly", readOnly ? "readonly" : "" },
            }
        };
    }
}