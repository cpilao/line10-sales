using MediatR;

namespace Line10.Sales.Application.Queries;

public sealed record GetProductRequest : IRequest<GetProductResponse>
{
    public Guid ProductId { get; init; }
}