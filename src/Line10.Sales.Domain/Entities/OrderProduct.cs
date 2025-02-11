namespace Line10.Sales.Domain.Entities;

public class OrderProduct
{
    public Guid OrderId { get; internal set; }
    public virtual Order Order { get; private set; } = null!;
    public Guid ProductId { get; internal set; }
    public virtual Product Product { get; private set; } = null!;
    public int Quantity { get; set; } 
}