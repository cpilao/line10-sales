using System.Security.Cryptography;

namespace Line10.Sales.Core.Security;

public class KeyGenerator
{
    public static string Generate256BitKey()
    {
        var key = new byte[32]; // 256 bits
        RandomNumberGenerator.Fill(key);
        return Convert.ToBase64String(key);
    }
}