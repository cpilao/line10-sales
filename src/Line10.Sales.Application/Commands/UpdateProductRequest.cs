using MediatR;

namespace Line10.Sales.Application.Commands;

public sealed record UpdateProductRequest : IRequest<VoidResponse>
{
    public Guid ProductId { get; init; }
    public string Name { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
    public string Sku { get; init; } = string.Empty;
}
