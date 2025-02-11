using Line10.Sales.Domain.Entities;
using Line10.Sales.Domain.Persistence;
using Line10.Sales.Domain.Projections;
using Microsoft.EntityFrameworkCore;

namespace Line10.Sales.Infrastructure.Repositories;

public class OrderRepository: Repository<Order>, IOrderRepository
{
    public OrderRepository(ApplicationDbContext context) :
        base(context)
    {
    }

    public async Task<IEnumerable<OrderProductProjection>> GetProducts(Guid orderId, CancellationToken cancellationToken = default)
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
            .ToListAsync(cancellationToken);
    }

    protected override void OnLoad(Order entity)
    {
        base.OnLoad(entity);
        _context.Entry(entity)
            .Collection(o => o.OrderProducts)
            .Load();
    }
}