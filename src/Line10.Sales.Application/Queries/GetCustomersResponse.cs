using Line10.Sales.Domain.Entities;

namespace Line10.Sales.Application.Queries;

public sealed record GetCustomersResponse: BaseResponse
{
    public IEnumerable<Customer> Customers { get; init; } = [];
}