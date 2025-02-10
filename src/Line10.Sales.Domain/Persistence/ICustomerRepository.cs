using Line10.Sales.Domain.Entities;

namespace Line10.Sales.Domain.Persistence;

public interface ICustomerRepository
{
    ValueTask<Customer?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    ValueTask AddAsync(Customer customer, CancellationToken cancellationToken = default);
    ValueTask UpdateAsync(Customer customer, CancellationToken cancellationToken = default);
    ValueTask DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}