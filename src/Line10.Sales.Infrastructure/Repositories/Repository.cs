using Line10.Sales.Domain.Entities;

namespace Line10.Sales.Infrastructure.Repositories;

public class Repository<TEntity>
    where TEntity: BaseEntity
{
    private readonly ApplicationDbContext _context;

    public Repository(ApplicationDbContext context)
    {
        _context = context;
    }
    
    public async ValueTask<TEntity?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Set<TEntity>().FindAsync([id], cancellationToken);
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

    public async ValueTask DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var customer = await _context.Set<TEntity>().FindAsync([id], cancellationToken);
        if (customer != null)
        {
            _context.Set<TEntity>().Remove(customer);
            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}