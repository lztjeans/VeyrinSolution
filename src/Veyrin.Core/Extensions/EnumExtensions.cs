using System.ComponentModel;
using System.Reflection;
using Veyrin.Core.Models;


public static class EnumExtensions
{
    public static string GetDescription(this Enum value)
    {
        var fi = value.GetType().GetField(value.ToString());
        if (fi == null) return value.ToString();
        var attr = fi.GetCustomAttribute<DescriptionAttribute>();
        return attr?.Description ?? value.ToString();
    }

    public static string ToYorN(this YesNoEnum yesNo) => ((int)yesNo % 2) == 0 ? "Y" : "N";

}