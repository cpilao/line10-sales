using Line10.Sales.Core;

namespace Line10.Sales.Domain.Entities;

public class Order: BaseEntity
{
    private const string InvalidOrderCustomerId = nameof(InvalidOrderCustomerId);
    private const string InvalidOrderProductId = nameof(InvalidOrderProductId);
    
    public Guid CustomerId { get; private set; }
    public Guid ProductId { get; private set; }
    public OrderStatus Status { get; private set; }
    public DateTime CreateDate { get; private set; }
    public DateTime? UpdateDate { get; private set; }

    private Order()
    {
    }

    private Order(
        Guid customerId,
        Guid productId)
    {
        Id = Guid.NewGuid();
        CustomerId = customerId;
        ProductId = productId;
        Status = OrderStatus.Pending;
        CreateDate = DateTime.Now;
    }

    public static Result<Order?> Create(
        Guid customerId,
        Guid productId)
    {
        var errors = new List<Error>();
        if (customerId == Guid.Empty)
        {
            errors.Add(new Error(InvalidOrderCustomerId));
        }

        if (productId == Guid.Empty)
        {
            errors.Add(new Error(InvalidOrderProductId));
        }

        return errors.Count != 0
            ? Result.Create<Order?>(errors.ToArray())
            : Result.Create<Order?>(new Order(customerId, productId));
    }
}