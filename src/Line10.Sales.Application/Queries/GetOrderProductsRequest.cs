using MediatR;

namespace Line10.Sales.Application.Queries;

public sealed record GetOrderProductsRequest : IRequest<GetOrderProductsResponse>
{
    public Guid OrderId { get; init; }
}