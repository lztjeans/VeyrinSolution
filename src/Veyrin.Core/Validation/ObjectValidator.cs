using System.Reflection;
using Veyrin.Core.Exceptions;

namespace Veyrin.Core.Validation;

public static class ObjectValidator
{
    /// <summary>
    /// 驗證物件上所有標有 VeyrinValidationAttribute 的屬性
    /// </summary>
    public static ValidationResult Validate(object instance)
    {
        Guard.NotNull(instance);
        var result = new ValidationResult();
        var type = instance.GetType();

        // 取得所有公開屬性
        var properties = type.GetProperties(BindingFlags.Public | BindingFlags.Instance);

        foreach (var prop in properties)
        {
            // 找出該屬性上所有繼承自 VeyrinValidationAttribute 的標籤
            var attributes = prop.GetCustomAttributes<ValidationAttribute>();
            var value = prop.GetValue(instance);

            foreach (var attr in attributes)
            {
                if (!attr.IsValid(value))
                {
                    result.Errors.Add(new ValidationError(
                        prop.Name,
                        attr.FormatErrorMessage(prop.Name)
                    ));
                }
            }
        }

        return result;
    }

    /// <summary>
    /// 驗證物件，若有錯誤則直接拋出集合異常
    /// </summary>
    public static void ValidateOrThrow(object instance)
    {
        var result = Validate(instance);
        if (!result.IsValid)
        {
            var combinedMessage = string.Join("; ", result.Errors.Select(e => e.Message));
            throw new ValidationException(combinedMessage);
        }
    }
}

