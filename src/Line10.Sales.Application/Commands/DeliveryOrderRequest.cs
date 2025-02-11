using MediatR;

namespace Line10.Sales.Application.Commands;

public sealed record DeliveryOrderRequest : IRequest<VoidResponse>
{
    public Guid OrderId { get; init; }
}