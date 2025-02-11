using Line10.Sales.Domain.Entities;
using Line10.Sales.Domain.Persistence;
using MediatR;

namespace Line10.Sales.Application.Queries;

public sealed record GetProductsRequest : IRequest<GetProductsResponse>
{
    public SortInfo<Product>? SortInfo { get; init; }
    public int PageNumber { get; init; }
    public int PageSize { get; init; }
    public string? Name { get; init; }
}