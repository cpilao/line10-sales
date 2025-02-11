using Line10.Sales.Domain.Entities;
using Line10.Sales.Domain.Persistence;

namespace Line10.Sales.Application.Queries;

public abstract record BaseQueryRequest<TEntity>
    where TEntity : BaseEntity
{
    public SortInfo<TEntity>? SortInfo { get; init; }
    public int PageNumber { get; init; }
    public int PageSize { get; init; }
}