using Line10.Sales.Domain.Entities;

namespace Line10.Sales.Application.Queries;

public sealed record GetOrderResponse: BaseResponse
{
    public Guid ProductId { get; init; }
    public Guid CustomerId { get; init; }
    public OrderStatus Status { get; init; }
}