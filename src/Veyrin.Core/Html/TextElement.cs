namespace Veyrin.Core.Html;

public class TextElement : AbstractElements
{
    public TextElement() { }

    public static TextElement Create(string id, string label, string name = "",
                                     string desc = "", bool isReq = true, bool readOnly = false)
    {
        return new TextElement
        {
            Id = $"txt_{id}",
            Name = StringUtils.IsEmpty(name) ? $"txt_{id}" : name,
            Label = label,
            Description = desc,
            IsRequired = isReq,
            Attributes = new Dictionary<string, string>() {
                { "class", "form-control" },
                { "style", "text-transform: uppercase;" },
                { "placeholder", "" },
                { "maxlength", "" },
                { "readonly", readOnly ? "readonly" : "" },
            }
        };
    }

    public static TextElement Create(string id, string label, string name)
    {
        return new TextElement
        {
            Id = $"txt_{id}",
            Label = label,
            Name = StringUtils.IsEmpty(name) ? $"txt_{id}" : name,
            Value = "",
            IsRequired = true,
            Attributes = new Dictionary<string, string>() {
                { "class", "form-control" },
                { "style", "text-transform: uppercase;" },
                { "placeholder", "" },
                { "maxlength", "" },
                { "readonly", "" },
            }
        };
    }
    public TextElement SetupStyle(string style, string css)
    {
        Attributes["style"] = style;
        Attributes["class"] = css;
        return this;
    }
    public TextElement AddAttribute(bool isReq = true, bool readOnly = false, int? maxleng = null, string desc = "", string defaultText = "")
    {
        IsRequired = isReq;
        Attributes["readonly"] = readOnly ? "readonly" : "";
        Attributes["placeholder"] = defaultText;
        Attributes["maxlength"] = maxleng == null ? "" : $"{maxleng}";
        Description = desc;
        return this;
    }

    public TextElement UpdAttribute(string key, string value)
    {
        Attributes[key] = value;
        return this;
    }



}



