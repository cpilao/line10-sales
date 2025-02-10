using MediatR;

namespace Line10.Sales.Application.Commands;

public sealed record DeleteProductRequest : IRequest<VoidResponse>
{
    public Guid ProductId { get; init; }
}