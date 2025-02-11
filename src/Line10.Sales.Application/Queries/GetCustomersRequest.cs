using Line10.Sales.Domain.Entities;
using Line10.Sales.Domain.Persistence;
using MediatR;

namespace Line10.Sales.Application.Queries;

public sealed record GetCustomersRequest : IRequest<GetCustomersResponse>
{
    public SortInfo<Customer>? SortInfo { get; init; }
    public int PageNumber { get; init; }
    public int PageSize { get; init; }
    public string? FirstName { get; init; }
    public string? LastName { get; init; }
}