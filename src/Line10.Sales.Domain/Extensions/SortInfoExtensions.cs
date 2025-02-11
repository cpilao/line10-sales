using Line10.Sales.Core;
using Line10.Sales.Domain.Entities;
using Line10.Sales.Domain.Persistence;

namespace Line10.Sales.Domain.Extensions;

public static class SortInfoExtensions
{
    public static SortInfo<T>? GetSortInfo<T>(this string? orderBy, SortOrder? order)
        where T: BaseEntity
    {
        return !string.IsNullOrEmpty(orderBy)
            ? new SortInfo<T>
            {
                Expression = ExpressionHelper.GetExpression<T>(orderBy),
                Order = order ?? SortOrder.Asc
            } : null;
    }
}