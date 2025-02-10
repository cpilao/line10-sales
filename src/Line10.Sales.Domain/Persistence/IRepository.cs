using Line10.Sales.Domain.Entities;

namespace Line10.Sales.Domain.Persistence;

public interface IRepository<TEntity>
    where TEntity: BaseEntity
{
    ValueTask<TEntity?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    ValueTask AddAsync(TEntity entity, CancellationToken cancellationToken = default);
    ValueTask UpdateAsync(TEntity entity, CancellationToken cancellationToken = default);
    ValueTask DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}