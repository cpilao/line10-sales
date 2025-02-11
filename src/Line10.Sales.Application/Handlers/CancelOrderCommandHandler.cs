using Line10.Sales.Application.Commands;
using Line10.Sales.Core;
using Line10.Sales.Domain.Persistence;
using MediatR;

namespace Line10.Sales.Application.Handlers;

public class CancelOrderCommandHandler: IRequestHandler<CancelOrderRequest, VoidResponse>
{
    private readonly IOrderRepository _orderRepository;

    public CancelOrderCommandHandler(IOrderRepository orderRepository)
    {
        _orderRepository = orderRepository;
    }
    
    public async Task<VoidResponse> Handle(
        CancelOrderRequest request,
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

        var result = order.Cancel();
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