namespace Line10.Sales.Application.Commands;

public sealed record CreateOrderResponse: BaseResponse
{
    public Guid? OrderId { get; init; }
}