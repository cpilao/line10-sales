using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.JsonWebTokens;

namespace Line10.Sales.Api.Security;

internal static class JwtBearerOptionsExtensions
{
    /// <summary>
    /// Bind JwtBearerOptions with configured TokenValidationOptions
    /// </summary>
    /// <param name="options"></param>
    /// <param name="tokenValidationOptions">TokenValidationOptions instance</param>
    internal static void Bind(
        this JwtBearerOptions options,
        TokenValidationOptions tokenValidationOptions)
    {
        options.TokenValidationParameters.ValidateLifetime = true;
        options.TokenValidationParameters.ValidAudience = tokenValidationOptions.Audience ??
                                                          options.Audience;
        options.TokenValidationParameters.ValidateAudience =
            !string.IsNullOrEmpty(options.TokenValidationParameters.ValidAudience);
        options.TokenValidationParameters.ValidIssuer = tokenValidationOptions.Issuer ??
                                                        options.TokenValidationParameters.ValidIssuer;
        options.TokenValidationParameters.ValidateIssuer =
            !string.IsNullOrEmpty(options.TokenValidationParameters.ValidIssuer);

        // hack to not require signature (just for demo purposes)
        // in a real production env this should be configured properly
        options.TokenValidationParameters.SignatureValidator =
            (token, _) => new JsonWebToken(token);
    }
}