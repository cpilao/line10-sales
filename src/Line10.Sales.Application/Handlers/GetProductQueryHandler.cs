using Line10.Sales.Application.Queries;
using Line10.Sales.Core;
using Line10.Sales.Domain.Persistence;
using MediatR;

namespace Line10.Sales.Application.Handlers;

public class GetProductQueryHandler: IRequestHandler<GetProductRequest, GetProductResponse>
{
    private readonly IProductRepository _productRepository;

    public GetProductQueryHandler(IProductRepository productRepository)
    {
        _productRepository = productRepository;
    }
    
    public async Task<GetProductResponse> Handle(
        GetProductRequest request,
        CancellationToken cancellationToken)
    {
        var product = await _productRepository.GetByIdAsync(request.ProductId, cancellationToken);

        if (product == null)
        {
            return new GetProductResponse
            {
                Errors = [new Error("NotFound")]
            };
        }
        
        return new GetProductResponse
        {
            Product = product
        };
    }
}