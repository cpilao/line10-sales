using Line10.Sales.Application.Commands;
using Line10.Sales.Core;
using Line10.Sales.Domain.Persistence;
using MediatR;

namespace Line10.Sales.Application.Handlers;

public class UpdateProductCommandHandler: IRequestHandler<UpdateProductRequest, VoidResponse>
{
    private readonly IProductRepository _productRepository;

    public UpdateProductCommandHandler(IProductRepository productRepository)
    {
        _productRepository = productRepository;
    }
    
    public async Task<VoidResponse> Handle(
        UpdateProductRequest request,
        CancellationToken cancellationToken)
    {
        var product = await _productRepository.GetByIdAsync(request.ProductId, cancellationToken);

        if (product == null)
        {
            return new VoidResponse
            {
                Errors = [new Error("NotFound")]
            };
        }

        product.UpdateName(request.Name);
        product.UpdateDescription(request.Description);
        product.UpdateSku(request.Sku);
        
        await _productRepository.UpdateAsync(product, cancellationToken);
        return VoidResponse.Success;
    }
}