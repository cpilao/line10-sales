using Line10.Sales.Application.Commands;
using Line10.Sales.Domain.Entities;
using Line10.Sales.Domain.Persistence;
using MediatR;

namespace Line10.Sales.Application.Handlers;

public class CreateOrderCommandHandler: IRequestHandler<CreateOrderRequest, CreateOrderResponse>
{
    private readonly IOrderRepository _orderRepository;

    public CreateOrderCommandHandler(IOrderRepository orderRepository)
    {
        _orderRepository = orderRepository;
    }
    
    public async Task<CreateOrderResponse> Handle(
        CreateOrderRequest request,
        CancellationToken cancellationToken)
    {
        var result = Order.Create(
            request.CustomerId,
            request.ProductId);

        if (result.HasErrors)
        {
            return new CreateOrderResponse
            {
                Errors = result.Errors
            };
        }
        
        await _orderRepository.AddAsync(result.Content!, cancellationToken);
        return new CreateOrderResponse
        {
            OrderId = result.Content!.Id
        };
    }
}