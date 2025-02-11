using Line10.Sales.Application.Queries;
using Line10.Sales.Domain.Persistence;
using MediatR;

namespace Line10.Sales.Application.Handlers;

public class GetOrderProductsQueryHandler: IRequestHandler<GetOrderProductsRequest, GetOrderProductsResponse>
{
    private readonly IOrderRepository _orderRepository;

    public GetOrderProductsQueryHandler(IOrderRepository orderRepository)
    {
        _orderRepository = orderRepository;
    }
    
    public async Task<GetOrderProductsResponse> Handle(
        GetOrderProductsRequest request,
        CancellationToken cancellationToken)
    {
        var orderProducts = await _orderRepository.GetProducts(request.OrderId, cancellationToken);

        return new GetOrderProductsResponse
        {
            OrderProducts = orderProducts
        };
    }
}