using Line10.Sales.Domain.Projections;

namespace Line10.Sales.Application.Queries;

public sealed record GetOrderProductsResponse: BaseResponse
{
    public IEnumerable<OrderProductProjection> OrderProducts { get; init; } = [];
}