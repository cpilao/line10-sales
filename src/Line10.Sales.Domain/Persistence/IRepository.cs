using System.Linq.Expressions;
using Line10.Sales.Domain.Entities;

namespace Line10.Sales.Domain.Persistence;

public interface IRepository<TEntity>
    where TEntity: BaseEntity
{
    ValueTask<TEntity?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    ValueTask AddAsync(TEntity entity, CancellationToken cancellationToken = default);
    ValueTask UpdateAsync(TEntity entity, CancellationToken cancellationToken = default);
    ValueTask<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
    ValueTask<IEnumerable<TEntity>> GetPage(
        int pageNumber,
        int pageSize,
        Expression<Func<TEntity, bool>>? filter = null,
        CancellationToken cancellationToken = default);
}