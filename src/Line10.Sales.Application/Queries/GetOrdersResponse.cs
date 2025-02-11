using Line10.Sales.Domain.Entities;

namespace Line10.Sales.Application.Queries;

public sealed record GetOrdersResponse: BaseResponse
{
    public IEnumerable<Order> Orders { get; init; } = [];
}