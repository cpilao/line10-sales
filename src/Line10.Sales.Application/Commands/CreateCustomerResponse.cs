namespace Line10.Sales.Application.Commands;

public sealed record CreateCustomerResponse: BaseResponse
{
    public Guid? CustomerId { get; init; }
}