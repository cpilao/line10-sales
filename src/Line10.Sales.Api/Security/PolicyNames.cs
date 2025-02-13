namespace Line10.Sales.Api.Security;

public static class PolicyNames
{
    public static readonly Policy CustomersRead = new(nameof(CustomersRead), "customers.read");
    public static readonly Policy CustomersWrite = new(nameof(CustomersWrite), "customers.write");
    public static readonly Policy ProductsRead = new(nameof(ProductsRead), "products.read");
    public static readonly Policy ProductsWrite = new(nameof(ProductsWrite), "products.write");
    public static readonly Policy OrdersRead = new(nameof(OrdersRead), "orders.read");
    public static readonly Policy OrdersWrite = new(nameof(OrdersWrite), "orders.write");
    
    public static readonly Policy[] All = [CustomersRead];
}