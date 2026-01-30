namespace Veyrin.Core.Html;
public class SelectElement : AbstractElements
{
    public List<string> Options { get; set; } = [];
    //public bool? IsMultiple { get; set; }
    public static SelectElement Create(string id, string label, string name = "", string desc = "", bool isReq = true, bool readOnly = false, bool isMulti = false, params string[] options)
    {
        return new SelectElement
        {
            Id = $"ddl_{id}",
            Name = StringUtils.IsEmpty(name) ? label : name,
            Label = label,
            Description = desc,
            IsRequired = isReq,
            Options = [.. options],
            Attributes = new Dictionary<string, string>() {
                { "class", "form-select fSelect" },
                { "style", "text-transform: uppercase;" },

                { "IsMultiple", isMulti? "true":"false" },
                { "multiple", isMulti? "multiple":"" },
                { "readonly", readOnly ? "disabled" : "" },
            },
            //ReadOnly = readOnly,
            //IsMultiple = isMulti,
        };
    }
    public SelectElement AddAttribute(bool isReq)
    {
        throw new NotImplementedException();
    }

    public SelectElement UpdAttribute(string key, string value)
    {
        Attributes[key] = value;
        return this;
    }

}