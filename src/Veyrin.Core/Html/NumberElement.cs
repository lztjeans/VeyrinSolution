namespace Veyrin.Core.Html;
public class NumberElement : AbstractElements
{
    //public int? Max { get; private set; }
    //public int? Min { get; private set; }

    public static NumberElement Create(string id, string label, string name, int? max = null, int? min = null, bool isReq = true, bool readOnly = false)
    {
        var e = new NumberElement
        {
            Id = $"txt_{id}",
            Name = name,
            Label = label,
            IsRequired = isReq,
            Attributes = new Dictionary<string, string>() {
                { "class", "form-control" },
                { "style", "text-transform: uppercase;" },
                { "inputMax", $"{max}" },
                { "inputMin", $"{min}" },
                { "placeholder", "" },
                { "readonly", readOnly ? "readonly" : "" },
                { "required", readOnly ? "required" : "" },
            }
            //Max = max,
            //Min = min
            //ReadOnly = readOnly,
        };
        return e;
    }
    public void SetValue(string _val) => Value = _val;
}