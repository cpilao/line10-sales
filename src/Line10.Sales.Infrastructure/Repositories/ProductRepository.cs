using Line10.Sales.Domain.Entities;
using Line10.Sales.Domain.Persistence;

namespace Line10.Sales.Infrastructure.Repositories;

public class ProductRepository: Repository<Product>, IProductRepository
{
    public ProductRepository(ApplicationDbContext context) :
        base(context)
    {
    }
}