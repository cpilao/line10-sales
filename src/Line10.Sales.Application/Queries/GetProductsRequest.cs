using MediatR;

namespace Line10.Sales.Application.Queries;

public sealed record GetProductsRequest : IRequest<GetProductsResponse>
{
    public int PageNumber { get; init; }
    public int PageSize { get; init; }
}