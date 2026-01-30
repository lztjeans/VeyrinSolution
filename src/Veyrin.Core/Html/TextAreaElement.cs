namespace Veyrin.Core.Html;
public class TextAreaElement : AbstractElements
{
    public static TextAreaElement Create(string id, string label, string name = "", string desc = "", bool isReq = true, bool readOnly = false)
    {
        return new TextAreaElement
        {
            Id = $"txt_{id}",
            Name = StringUtils.IsEmpty(name) ? label : name,
            Label = label,
            IsRequired = isReq,
            Description = desc,
            Attributes = new Dictionary<string, string>() {
                { "class", "form-control" },
                { "style", "text-transform: uppercase;" },
                { "readonly", readOnly ? "readonly" : "" },
            }
            //ReadOnly = readOnly,
        };
    }
}