namespace Line10.Sales.Core;

public sealed class Error
{
    public string Code { get; }

    public string? Message { get; }

    public Dictionary<string, string> Metadata { get; }

    public Error(
        string code,
        string? message = null,
        Dictionary<string, string>? metadata = null)
    {
        Code = code ?? throw new ArgumentNullException(nameof(code));
        Message = message ?? code;
        Metadata = metadata ?? new Dictionary<string, string>();
    }

    public static implicit operator string(Error error) => error.Code;

    public static explicit operator Error(string errorCode) => new(errorCode);
}