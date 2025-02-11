namespace Line10.Sales.Domain.Projections;

public sealed record OrderProductProjection
{
    public required Guid OrderId { get; init; }
    public required Guid ProductId { get; init; }
    public required string ProductName { get; init; }
    public required string ProductDescription { get; init; }
    public required int Quantity { get; init; }
}