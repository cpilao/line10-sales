using MediatR;

namespace Line10.Sales.Application.Commands;

public sealed record AddOrderProductRequest : IRequest<VoidResponse>
{
    public Guid OrderId { get; init; }
    public Guid ProductId { get; init; }
    public int Quantity { get; init; }
}