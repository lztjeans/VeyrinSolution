namespace Veyrin.Core.Exceptions;

public class ValidationException : Exception
{
    public string? PropertyName { get; }

    public ValidationException(string message) : base(message) { }

    public ValidationException(string message, string propertyName)
        : base($"{message} (Property: {propertyName})")
    {
        PropertyName = propertyName;
    }
}