using Line10.Sales.Domain.Entities;
using MediatR;

namespace Line10.Sales.Application.Queries;

public sealed record GetCustomerRequest : IRequest<GetCustomerResponse>
{
    public Guid CustomerId { get; init; }
}

public sealed record GetCustomersRequest : IRequest<GetCustomersResponse>
{
    public int PageNumber { get; init; }
    public int PageSize { get; init; }
}

public sealed record GetCustomersResponse: BaseResponse
{
    public IEnumerable<Customer> Customers { get; init; } = [];
}