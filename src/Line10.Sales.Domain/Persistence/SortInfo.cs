using System.Linq.Expressions;
using Line10.Sales.Domain.Entities;

namespace Line10.Sales.Domain.Persistence;

public sealed record SortInfo<TEntity>
    where TEntity: BaseEntity
{
    public required Expression<Func<TEntity, object>> Expression { get; init; }
    public SortOrder Order { get; init; } 
}