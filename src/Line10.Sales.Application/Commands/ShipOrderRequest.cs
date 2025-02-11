using MediatR;

namespace Line10.Sales.Application.Commands;

public sealed record ShipOrderRequest : IRequest<VoidResponse>
{
    public Guid OrderId { get; init; }
}