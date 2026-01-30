namespace Veyrin.Core.Html;

public abstract class AbstractElements
{
    public string Label { get; protected set; } = string.Empty;
    public string Id { get; protected set; } = string.Empty;
    public string? Name { get; protected set; }
    public string? Value { get; protected set; }
    public string Description { get; protected set; } = string.Empty;
    public bool IsRequired { get; protected set; } = true; //  是否必填
    protected Dictionary<string, string> Attributes { get; set; } = [];

    public string GetAttributes(string name)
    {
        Attributes.TryGetValue(name, out var attr);
        return attr ?? string.Empty;
    }

}


//public string? Style { get; protected set; }
//public string? CssName { get; protected set; }

//public bool ReadOnly { get; protected set; } = true;


//public T AddAttribute(string key, string value)
//{
//    Attributes[key] = value;
//    return this;
//}
//public T SetupStyle<T>(this T _t, string style, string css)
//{
//    Style = style;
//    CssName = css;
//    return _t;
//}