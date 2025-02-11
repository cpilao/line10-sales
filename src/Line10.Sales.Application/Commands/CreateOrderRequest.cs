using MediatR;

namespace Line10.Sales.Application.Commands;

public sealed record CreateOrderRequest : IRequest<CreateOrderResponse>
{
    public Guid ProductId { get; init; }
    public Guid CustomerId { get; init; }
}