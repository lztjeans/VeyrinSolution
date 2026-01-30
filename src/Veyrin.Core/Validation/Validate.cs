using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Veyrin.Core.Validation;

/// <summary>
/// 封裝單個驗證錯誤
/// </summary>
public record ValidationError(string PropertyName, string Message, string? ErrorCode = null);

/// <summary>
/// 驗證結果載體
/// </summary>
public class ValidationResult
{
    public bool IsValid => Errors.Count == 0;
    public List<ValidationError> Errors { get; } = new();

    public static ValidationResult Success() => new();
    public static ValidationResult Failure(string prop, string msg)
    {
        var res = new ValidationResult();
        res.Errors.Add(new ValidationError(prop, msg));
        return res;
    }
}

/// <summary>
/// 驗證介面：供 Veyrin.Data 或實體模型實作
/// </summary>
public interface IValidator<in T>
{
    ValidationResult Validate(T instance);
}
