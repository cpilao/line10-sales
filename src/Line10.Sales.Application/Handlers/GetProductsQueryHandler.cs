using System.Linq.Expressions;
using Line10.Sales.Application.Queries;
using Line10.Sales.Core.Extensions;
using Line10.Sales.Domain.Entities;
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
        Expression<Func<Product, bool>> filter = o => true;

        if (!string.IsNullOrEmpty(request.Name))
        {
            filter = filter.And(o => o.Name.Contains(request.Name));
        }
        
        var products = await _productRepository.GetPage(
            request.PageNumber, 
            request.PageSize,
            filter: filter,
            sortInfo: request.SortInfo,
            cancellationToken: cancellationToken);

        return new GetProductsResponse
        {
            Products = products
        };
    }
}