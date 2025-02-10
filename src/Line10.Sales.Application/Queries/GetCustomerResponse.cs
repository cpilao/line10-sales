using Line10.Sales.Domain.Entities;

namespace Line10.Sales.Application.Queries;

public sealed record GetCustomerResponse: BaseResponse
{
    public Customer? Customer { get; init; }
}