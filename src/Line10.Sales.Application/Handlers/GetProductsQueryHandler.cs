using Line10.Sales.Application.Queries;
using Line10.Sales.Domain.Persistence;
using MediatR;

namespace Line10.Sales.Application.Handlers;

public class GetProductsQueryHandler: IRequestHandler<GetProductsRequest, GetProductsResponse>
{
    private readonly IProductRepository _productRepository;

    public GetProductsQueryHandler(IProductRepository productRepository)
    {
        _productRepository = productRepository;
    }
    
    public async Task<GetProductsResponse> Handle(
        GetProductsRequest request,
        CancellationToken cancellationToken)
    {
        var products = await _productRepository.GetPage(
            request.PageNumber, 
            request.PageSize,
            cancellationToken: cancellationToken);

        return new GetProductsResponse
        {
            Products = products
        };
    }
}