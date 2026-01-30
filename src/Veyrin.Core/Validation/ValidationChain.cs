using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.CompilerServices;
using Veyrin.Core.Exceptions;

namespace Veyrin.Core.Validation;

public class ValidationChain<T>
{
    private readonly T _value;
    private readonly string _paramName;
    private readonly List<string> _errors = new();

    public ValidationChain(T value, string paramName)
    {
        _value = value;
        _paramName = paramName;
    }

    public ValidationChain<T> IsNotNull()
    {
        if (_value == null) _errors.Add($"{_paramName} cannot be null.");
        return this;
    }

    // 整合 Common Rules
    public ValidationChain<T> Matches(string pattern, string message)
    {
        if (_value is string s && !StringUtils.IsMatch(s, pattern, out _))
            _errors.Add($"{_paramName}: {message}");
        return this;
    }

    public void ThrowIfInvalid()
    {
        if (_errors.Count > 0)
            throw new ValidationException(string.Join(" ", _errors), _paramName);
    }

    public bool IsValid() => _errors.Count == 0;
}

// 入口點
public static partial class Guard
{
    public static ValidationChain<T> Check<T>(T value, [CallerArgumentExpression("value")] string paramName = "")
        => new ValidationChain<T>(value, paramName);
}
