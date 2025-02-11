using MediatR;

namespace Line10.Sales.Application.Queries;

public sealed record GetOrderRequest : IRequest<GetOrderResponse>
{
    public Guid OrderId { get; init; }
}