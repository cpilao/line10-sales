namespace Line10.Sales.Infrastructure.Projections;

public class OrderProductProjection
{
    public required Guid OrderId { get; init; }
    public required Guid ProductId { get; init; }
    public required string ProductName { get; init; }
    public required string ProductDescription { get; init; }
    
    public required int Quantity { get; init; }
}