using Line10.Sales.Application.Commands;
using Line10.Sales.Domain.Entities;
using Line10.Sales.Domain.Persistence;
using MediatR;

namespace Line10.Sales.Application.Handlers;

public class CreateProductCommandHandler: IRequestHandler<CreateProductRequest, CreateProductResponse>
{
    private readonly IProductRepository _productRepository;

    public CreateProductCommandHandler(IProductRepository productRepository)
    {
        _productRepository = productRepository;
    }
    
    public async Task<CreateProductResponse> Handle(
        CreateProductRequest request,
        CancellationToken cancellationToken)
    {
        var result = Product.Create(
            request.Name,
            request.Description,
            request.Sku);

        if (result.HasErrors)
        {
            return new CreateProductResponse
            {
                Errors = result.Errors
            };
        }
        
        await _productRepository.AddAsync(result.Content!, cancellationToken);
        return new CreateProductResponse
        {
            ProductId = result.Content!.Id
        };
    }
}