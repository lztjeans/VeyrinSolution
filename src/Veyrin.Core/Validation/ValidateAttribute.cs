using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Veyrin.Core.Validation;

[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
public abstract class ValidationAttribute : Attribute
{
    public string ErrorMessage { get; set; } = "Validation failed.";
    public abstract bool IsValid(object? value);
    // 格式化錯誤訊息
    public virtual string FormatErrorMessage(string name)
        => ErrorMessage ?? $"{name} validation failed.";
}

public class RequiredAttribute : ValidationAttribute
{
    public override bool IsValid(object? value) => value != null;
}

public class MaxLengthAttribute : ValidationAttribute
{
    public int Length { get; }
    public MaxLengthAttribute(int length) => Length = length;
    public override bool IsValid(object? value) => value is string s && s.Length <= Length;
}
public class PathAttribute : ValidationAttribute
{
    public override bool IsValid(object? value) => value is string path && !path.Any(c => Path.GetInvalidPathChars().Contains(c));
}

public class IpAttribute : ValidationAttribute
{
    public override bool IsValid(object? value) => value is string ip && System.Net.IPAddress.TryParse(ip, out _);
}