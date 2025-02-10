namespace Line10.Sales.Application.Commands;

public sealed record CreateProductResponse: BaseResponse
{
    public Guid? ProductId { get; init; }
}