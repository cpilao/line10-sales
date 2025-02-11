namespace Line10.Sales.Domain.Entities;

public class OrderProduct
{
    public Guid OrderId { get; internal set; }
    public Order Order { get; private set; } = null!;
    public Guid ProductId { get; internal set; }
    public Product Product { get; private set; } = null!;
    public int Quantity { get; set; } 
}