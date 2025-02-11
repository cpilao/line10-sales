using Line10.Sales.Application.Commands;
using Line10.Sales.Core;
using Line10.Sales.Domain.Persistence;
using MediatR;

namespace Line10.Sales.Application.Handlers;

public class RemoveOrderProductCommandHandler: IRequestHandler<RemoveOrderProductRequest, VoidResponse>
{
    private readonly IOrderRepository _orderRepository;
    private readonly IProductRepository _productRepository;

    public RemoveOrderProductCommandHandler(IOrderRepository orderRepository, IProductRepository productRepository)
    {
        _orderRepository = orderRepository;
        _productRepository = productRepository;
    }
    
    public async Task<VoidResponse> Handle(
        RemoveOrderProductRequest request,
        CancellationToken cancellationToken)
    {
        var order = await _orderRepository.GetByIdAsync(request.OrderId, cancellationToken);

        if (order == null)
        {
            return new VoidResponse
            {
                Errors = [new Error("NotFound", "Order not found")]
            };
        }
        
        var product = await _productRepository.GetByIdAsync(request.ProductId, cancellationToken);

        if (product == null)
        {
            return new VoidResponse
            {
                Errors = [new Error("NotFound", "Product not found")]
            };
        }

        var result = order.RemoveProduct(request.ProductId, request.Quantity);
        if (result.HasErrors)
        {
            return new VoidResponse
            {
                Errors = result.Errors
            };
        }
        
        await _orderRepository.UpdateAsync(order, cancellationToken);
        return VoidResponse.Success;
    }
}