using Microsoft.AspNetCore.OutputCaching;

namespace Line10.Sales.Api.Cache;

public sealed class ByIdCachePolicy : BaseCachePolicy
{
    public override ValueTask CacheRequestAsync(OutputCacheContext context, CancellationToken cancellationToken)
    {
        var id = context.HttpContext.Request.RouteValues["id"];
        if (id is null)
            return ValueTask.CompletedTask;

        context.Tags.Add(id.ToString()!);

        var attemptOutputCaching = AttemptOutputCaching(context);
        context.EnableOutputCaching = true;
        context.AllowCacheLookup = attemptOutputCaching;
        context.AllowCacheStorage = attemptOutputCaching;
        context.AllowLocking = true;

        // Vary by any query by default
        context.CacheVaryByRules.QueryKeys = "*";

        return ValueTask.CompletedTask;
    }
}