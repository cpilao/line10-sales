using Line10.Sales.Domain.Entities;

namespace Line10.Sales.Application.Queries;

public sealed record GetProductResponse: BaseResponse
{
    public Product? Product { get; init; }
}