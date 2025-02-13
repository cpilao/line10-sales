using Line10.Sales.Domain.Entities;
using Line10.Sales.Domain.Persistence;

namespace Line10.Sales.Infrastructure.Repositories;

public class CustomerRepository: Repository<Customer>, ICustomerRepository
{
    public CustomerRepository(ApplicationDbContext context) :
        base(context)
    {
    }
}