using Line10.Sales.Application.Commands;
using Line10.Sales.Core;
using Line10.Sales.Domain.Persistence;
using MediatR;

namespace Line10.Sales.Application.Handlers;

public class ProcessOrderCommandHandler: IRequestHandler<ProcessOrderRequest, VoidResponse>
{
    private readonly IOrderRepository _orderRepository;

    public ProcessOrderCommandHandler(IOrderRepository orderRepository)
    {
        _orderRepository = orderRepository;
    }
    
    public async Task<VoidResponse> Handle(
        ProcessOrderRequest request,
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

        var result = order.Process();
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