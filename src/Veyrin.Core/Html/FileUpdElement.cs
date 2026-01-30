namespace Veyrin.Core.Html;
public class FileUpdElement : AbstractElements
{
    //public bool IsMultiple { get;  set; }=false;
    //public string Accept { get; set; } = "*";

    public static FileUpdElement Create(string id, string label, string name)
    // = "", string desc = "", string accept = "*", bool multiple = false, bool isReq = true)
    {
        return new FileUpdElement
        {
            Id = id,
            Label = label,
            Name = StringUtils.IsEmpty(name) ? label : name,
            //Description = desc,
            //IsRequired = isReq,
            Attributes = new Dictionary<string, string>() {
                { "class", "form-control" },
                { "style", "display: none;" },
                //{ "readonly", readOnly ? "readonly" : "" },
            }
            //IsMultiple = multiple,
            //ReadOnly = readOnly,
            //Accept = accept

        };
    }
    public FileUpdElement AddAttribute(string desc = "", string accept = "*", bool multiple = false, bool isReq = true, bool canDwn = true)
    {
        base.Attributes.Add("hyperlnk", canDwn ? "Y" : "N");
        base.Attributes.Add("accept", accept);
        base.Attributes.Add("multiple", multiple ? "multiple" : "");
        Description = desc;
        IsRequired = isReq;
        return this;
    }

    public FileUpdElement AddStyle(string css = "form-control", string style = "display: none;")
    {
        base.Attributes.Add("class", css);
        base.Attributes.Add("style", style);
        return this;
    }

}