using Line10.Sales.Application.Commands;
using Line10.Sales.Core;
using Line10.Sales.Domain.Persistence;
using MediatR;

namespace Line10.Sales.Application.Handlers;

public class DeleteProductQueryHandler: IRequestHandler<DeleteProductRequest, VoidResponse>
{
    private readonly IProductRepository _productRepository;

    public DeleteProductQueryHandler(IProductRepository productRepository)
    {
        _productRepository = productRepository;
    }
    
    public async Task<VoidResponse> Handle(
        DeleteProductRequest request,
        CancellationToken cancellationToken)
    {
        var result = await _productRepository.DeleteAsync(request.ProductId, cancellationToken);
        return result
            ? VoidResponse.Success
            : new VoidResponse
            {
                Errors = [new Error("NotFound")]
            };
    }
}