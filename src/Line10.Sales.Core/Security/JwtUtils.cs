namespace Line10.Sales.Core.Security;

public class JwtUtils
{
    public static string GetToken(string[] roles)
    {
        var secretKey = KeyGenerator.Generate256BitKey();
        var issuer = "your-issuer";
        var audience = "your-audience";
        var userId = "user-id";
        var userName = "John Doe";
        return JwtTokenGenerator.GenerateToken(secretKey, issuer, audience, userId, userName, roles);
    }
}