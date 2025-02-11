using Line10.Sales.Domain.Entities;
using MediatR;

namespace Line10.Sales.Application.Queries;

public sealed record GetCustomersRequest : BaseQueryRequest<Customer>, IRequest<GetCustomersResponse>
{
    public string? FirstName { get; init; }
    public string? LastName { get; init; }
}