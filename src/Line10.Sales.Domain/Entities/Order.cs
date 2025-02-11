using Line10.Sales.Core;

namespace Line10.Sales.Domain.Entities;

public class Order: BaseEntity
{
    private const string InvalidOrderCustomerId = nameof(InvalidOrderCustomerId);
    private const string InvalidOrderProductId = nameof(InvalidOrderProductId);
    private const string InvalidOrderProductQuantity = nameof(InvalidOrderProductQuantity);
    private const string OrderProductNotFound= nameof(OrderProductNotFound);
    private const string OrderActionNotAllowed = nameof(OrderActionNotAllowed);
    
    public Guid CustomerId { get; private set; }
    public OrderStatus Status { get; private set; }
    public DateTime CreateDate { get; private set; }
    public DateTime? UpdateDate { get; private set; }
    public ICollection<OrderProduct> OrderProducts { get; private set; } = [];

    private Order()
    {
    }

    private Order(
        Guid customerId,
        Guid productId)
    {
        Id = Guid.NewGuid();
        CustomerId = customerId;
        OrderProducts = new List<OrderProduct>([new OrderProduct
        {
            ProductId = productId,
            OrderId = Id,
            Quantity = 1
        }]);
        Status = OrderStatus.Pending;
        CreateDate = DateTime.UtcNow;
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
    
    public Result Process()
    {
        if (Status != OrderStatus.Pending)
        {
            return Result.Create(new Error(OrderActionNotAllowed, "Orders can only be processed from 'Pending' state"));
        }
        
        Status = OrderStatus.Processing;
        UpdateDate = DateTime.UtcNow;
        return Result.Success;
    }
    
    public Result Ship()
    {
        if (Status != OrderStatus.Processing)
        {
            return Result.Create(new Error(OrderActionNotAllowed, "Orders can only be shipped from 'Processing' state"));
        }
        
        Status = OrderStatus.Shipped;
        UpdateDate = DateTime.UtcNow;
        return Result.Success;
    }
    
    public Result Delivery()
    {
        if (Status != OrderStatus.Shipped)
        {
            return Result.Create(new Error(OrderActionNotAllowed, "Orders can only be delivered from 'Shipped' state"));
        }
        
        Status = OrderStatus.Delivered;
        UpdateDate = DateTime.UtcNow;
        return Result.Success;
    }
    
    public Result Cancel()
    {
        Status = OrderStatus.Cancelled;
        UpdateDate = DateTime.UtcNow;
        return Result.Success;
    }
    
    public Result AddProduct(
        Guid productId,
        int quantity)
    {
        if (Status != OrderStatus.Pending)
        {
            return Result.Create(new Error(OrderActionNotAllowed, "Orders can only add new products in 'Pending' state"));
        }
        
        if (quantity <= 0)
        {
            return Result.Create(new Error(InvalidOrderProductQuantity));
        }

        var orderProduct = OrderProducts.FirstOrDefault(o => o.ProductId.Equals(productId));
        if (orderProduct == null)
        {
            OrderProducts.Add(new OrderProduct
            {
                OrderId = Id,
                ProductId = productId,
                Quantity = quantity
            });
        }
        else
        {
            orderProduct.Quantity += quantity;
        }

        UpdateDate = DateTime.UtcNow;
        return Result.Success;
    }
    
    public Result RemoveProduct(
        Guid productId,
        int? quantity)
    {
        if (Status != OrderStatus.Pending)
        {
            return Result.Create(new Error(OrderActionNotAllowed, "Orders can only add new products in 'Pending' state"));
        }
        
        if (quantity is <= 0)
        {
            return Result.Create(new Error(InvalidOrderProductQuantity));
        }
        
        var orderProduct = OrderProducts.FirstOrDefault(o => o.ProductId.Equals(productId));
        if (orderProduct == null)
        {
            return Result.Create(new Error(OrderProductNotFound));
        }

        // remove product from order
        if (!quantity.HasValue)
        {
            OrderProducts.Remove(orderProduct);
        }
        else
        {
            // remove product quantity from order
            orderProduct.Quantity -= quantity.Value;
            if (orderProduct.Quantity <= 0)
            {
                OrderProducts.Remove(orderProduct);
            }
        }

        UpdateDate = DateTime.UtcNow;
        return Result.Success;
    }
}