using Line10.Sales.Domain.Entities;

namespace Line10.Sales.Application.Queries;

public sealed record GetProductsResponse: BaseResponse
{
    public IEnumerable<Product> Products { get; init; } = [];
}