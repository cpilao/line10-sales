using Line10.Sales.Domain.Entities;
using Line10.Sales.Domain.Projections;

namespace Line10.Sales.Domain.Persistence;

public interface IOrderRepository : IRepository<Order>
{
    Task<IEnumerable<OrderProductProjection>> GetProducts(Guid orderId, CancellationToken cancellationToken = default);
}