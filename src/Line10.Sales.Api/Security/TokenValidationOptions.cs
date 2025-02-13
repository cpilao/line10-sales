namespace Line10.Sales.Api.Security;

public class TokenValidationOptions
{
    public string? Audience { get; set; }
    public string? Issuer { get; set; } = string.Empty;
}