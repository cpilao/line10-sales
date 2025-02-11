using Line10.Sales.Application.Commands;
using Line10.Sales.Core;
using Line10.Sales.Domain.Persistence;
using MediatR;

namespace Line10.Sales.Application.Handlers;

public class DeliveryOrderCommandHandler: IRequestHandler<DeliveryOrderRequest, VoidResponse>
{
    private readonly IOrderRepository _orderRepository;

    public DeliveryOrderCommandHandler(IOrderRepository orderRepository)
    {
        _orderRepository = orderRepository;
    }
    
    public async Task<VoidResponse> Handle(
        DeliveryOrderRequest request,
        CancellationToken cancellationToken)
    {
        var order = await _orderRepository.GetByIdAsync(request.OrderId, cancellationToken);

        if (order == null)
        {
            return new VoidResponse
            {
                Errors = [new Error("NotFound")]
            };
        }

        var result = order.Delivery();
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