using Microsoft.AspNetCore.OutputCaching;

namespace Line10.Sales.Api.Cache;

public sealed class ByRequestPathCachePolicy : BaseCachePolicy
{
    /// <inheritdoc />
    public override ValueTask CacheRequestAsync(OutputCacheContext context, CancellationToken cancellationToken)
    {
        var requestPath = context.HttpContext.Request.Path.ToString();
        if (string.IsNullOrEmpty(requestPath))
            return ValueTask.CompletedTask;

        context.Tags.Add(requestPath);

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