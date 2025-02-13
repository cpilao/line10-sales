using Microsoft.AspNetCore.Authorization;

namespace Line10.Sales.Api.Security;

public static class AuthorizationOptionsExtensions
{
    public static AuthorizationOptions AddPolicies(this AuthorizationOptions authorizationOptions)
    {
        foreach (var policy in PolicyNames.All)
        {
            authorizationOptions.AddPolicy(policy.Name, builder => builder.RequireRole(policy.Role));
        }

        return authorizationOptions;
    }
}