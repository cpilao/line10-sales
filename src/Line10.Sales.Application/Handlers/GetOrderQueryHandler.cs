using Line10.Sales.Application.Queries;
using Line10.Sales.Core;
using Line10.Sales.Domain.Persistence;
using MediatR;

namespace Line10.Sales.Application.Handlers;

public class GetOrderQueryHandler: IRequestHandler<GetOrderRequest, GetOrderResponse>
{
    private readonly IOrderRepository _orderRepository;

    public GetOrderQueryHandler(IOrderRepository orderRepository)
    {
        _orderRepository = orderRepository;
    }
    
    public async Task<GetOrderResponse> Handle(
        GetOrderRequest request,
        CancellationToken cancellationToken)
    {
        var order = await _orderRepository.GetByIdAsync(request.OrderId, cancellationToken);

        if (order == null)
        {
            return new GetOrderResponse
            {
                Errors = [new Error("NotFound")]
            };
        }

        return new GetOrderResponse
        {
            CustomerId = order.CustomerId,
            Status = order.Status
        };
    }
}