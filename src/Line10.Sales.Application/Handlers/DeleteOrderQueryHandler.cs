using Line10.Sales.Application.Commands;
using Line10.Sales.Core;
using Line10.Sales.Domain.Persistence;
using MediatR;

namespace Line10.Sales.Application.Handlers;

public class DeleteOrderQueryHandler: IRequestHandler<DeleteOrderRequest, VoidResponse>
{
    private readonly IOrderRepository _orderRepository;

    public DeleteOrderQueryHandler(IOrderRepository orderRepository)
    {
        _orderRepository = orderRepository;
    }
    
    public async Task<VoidResponse> Handle(
        DeleteOrderRequest request,
        CancellationToken cancellationToken)
    {
        var result = await _orderRepository.DeleteAsync(request.OrderId, cancellationToken);
        return result
            ? VoidResponse.Success
            : new VoidResponse
            {
                Errors = [new Error("NotFound")]
            };
    }
}