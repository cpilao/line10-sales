using Line10.Sales.Domain.Entities;
using Line10.Sales.Domain.Persistence;
using Line10.Sales.Infrastructure.Projections;
using Microsoft.EntityFrameworkCore;

namespace Line10.Sales.Infrastructure.Repositories;

public class OrderRepository: Repository<Order>, IOrderRepository
{
    public OrderRepository(ApplicationDbContext context) :
        base(context)
    {
    }

    public async Task<IEnumerable<OrderProductProjection>> GetProducts(Guid orderId)
    {
        return await _context.OrderProducts
            .Where(o => o.OrderId == orderId)
            .Select(op => new OrderProductProjection
            {
                OrderId = op.OrderId,
                ProductId = op.ProductId,
                ProductName = op.Product.Name,
                ProductDescription = op.Product.Description,
                Quantity = op.Quantity
            })
            .ToListAsync();
    }
}