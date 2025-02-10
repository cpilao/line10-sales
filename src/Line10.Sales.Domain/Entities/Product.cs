using Line10.Sales.Core;

namespace Line10.Sales.Domain.Entities;

public class Product: BaseEntity
{
    private const string InvalidProductName = nameof(InvalidProductName);
    private const string InvalidProductDescription = nameof(InvalidProductDescription);
    private const string InvalidProductSku = nameof(InvalidProductSku);
    
    public string Name { get; private set; } = string.Empty;
    public string Description { get; private set; } = string.Empty;
    public string Sku { get; private set; } = string.Empty;

    private Product()
    {
    }

    private Product(
        string name,
        string description,
        string sku)
    {
        Id = Guid.NewGuid();
        Name = name;
        Description = description;
        Sku = sku;
    }
    
    public Result UpdateName(string name)
    {
        var errors = new List<Error>();
        if (string.IsNullOrEmpty(name))
        {
            errors.Add(new Error(InvalidProductName));
        }

        if (errors.Count != 0)
        {
            return Result.Create(errors);
        }
        
        Name = name;
        return Result.Success;
    }
    
    public Result UpdateDescription(string description)
    {
        var errors = new List<Error>();
        if (string.IsNullOrEmpty(description))
        {
            errors.Add(new Error(InvalidProductDescription));
        }

        if (errors.Count != 0)
        {
            return Result.Create(errors);
        }

        Description = description;
        return Result.Success;
    }

    public Result UpdateSku(string sku)
    {
        var errors = new List<Error>();
        if (string.IsNullOrEmpty(sku))
        {
            errors.Add(new Error(InvalidProductSku));
        }

        if (errors.Count != 0)
        {
            return Result.Create(errors);
        }

        Sku = sku;
        return Result.Success;
    }

    public static Result<Product?> Create(
        string name,
        string description,
        string sku)
    {
        var errors = new List<Error>();
        if (string.IsNullOrEmpty(name))
        {
            errors.Add(new Error(InvalidProductName));
        }
        if (string.IsNullOrEmpty(description))
        {
            errors.Add(new Error(InvalidProductDescription));
        }
        if (string.IsNullOrEmpty(sku))
        {
            errors.Add(new Error(InvalidProductSku));
        }

        return errors.Count != 0
            ? Result.Create<Product?>(errors.ToArray())
            : Result.Create<Product?>(new Product(name, description, sku));
    }
}