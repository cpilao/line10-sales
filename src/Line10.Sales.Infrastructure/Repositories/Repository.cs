using System.Linq.Expressions;
using Line10.Sales.Domain.Entities;
using Line10.Sales.Domain.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Line10.Sales.Infrastructure.Repositories;

public class Repository<TEntity>
    where TEntity: BaseEntity
{
    protected readonly ApplicationDbContext _context;

    public Repository(ApplicationDbContext context)
    {
        _context = context;
    }
    
    public async ValueTask<TEntity?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var entity = await _context.Set<TEntity>().FindAsync([id], cancellationToken);
        if (entity != null)
        {
            OnLoad(entity);
        }
        return entity;
    }

    public async ValueTask AddAsync(TEntity customer, CancellationToken cancellationToken = default)
    {
        _context.Set<TEntity>().Add(customer);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async ValueTask UpdateAsync(TEntity customer, CancellationToken cancellationToken = default)
    {
        _context.Set<TEntity>().Update(customer);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async ValueTask<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var entity = await _context.Set<TEntity>().FindAsync([id], cancellationToken);
        if (entity != null)
        {
            _context.Set<TEntity>().Remove(entity);
            await _context.SaveChangesAsync(cancellationToken);
            return true;
        }

        return false;
    }

    public async ValueTask<IEnumerable<TEntity>> GetPage(
        int pageNumber,
        int pageSize,
        Expression<Func<TEntity, bool>>? filter = null,
        SortInfo<TEntity>? sortInfo = null,
        CancellationToken cancellationToken = default)
    {
        filter ??= entity => true;
        sortInfo ??= new SortInfo<TEntity>
        {
            Expression = entity => entity.Id,
            Order = SortOrder.Asc
        };

        var query = _context.Set<TEntity>()
            .Where(filter);

        if (sortInfo.Order == SortOrder.Asc)
        {
            query = query.OrderBy(sortInfo.Expression);
        }
        else
        {
            query = query.OrderByDescending(sortInfo.Expression);
        }
        
        return await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);
    }

    protected virtual void OnLoad(TEntity entity)
    {
        
    }
}