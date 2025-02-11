using Line10.Sales.Domain.Entities;
using MediatR;

namespace Line10.Sales.Application.Queries;

public sealed record GetProductsRequest : BaseQueryRequest<Product>, IRequest<GetProductsResponse>
{
    public string? Name { get; init; }
    public string? Description { get; init; }
    public string? Sku { get; init; }
}