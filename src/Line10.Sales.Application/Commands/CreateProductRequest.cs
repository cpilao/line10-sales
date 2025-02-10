using MediatR;

namespace Line10.Sales.Application.Commands;

public sealed record CreateProductRequest : IRequest<CreateProductResponse>
{
    public string Name { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
    public string Sku { get; init; } = string.Empty;
}