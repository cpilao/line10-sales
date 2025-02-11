using MediatR;

namespace Line10.Sales.Application.Commands;

public sealed record DeleteOrderRequest : IRequest<VoidResponse>
{
    public Guid OrderId { get; init; }
}