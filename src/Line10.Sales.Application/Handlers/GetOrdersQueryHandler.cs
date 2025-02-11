using System.Linq.Expressions;
using Line10.Sales.Application.Queries;
using Line10.Sales.Core.Extensions;
using Line10.Sales.Domain.Entities;
using Line10.Sales.Domain.Persistence;
using MediatR;

namespace Line10.Sales.Application.Handlers;

public class GetOrdersQueryHandler: IRequestHandler<GetOrdersRequest, GetOrdersResponse>
{
    private readonly IOrderRepository _orderRepository;

    public GetOrdersQueryHandler(IOrderRepository orderRepository)
    {
        _orderRepository = orderRepository;
    }
    
    public async Task<GetOrdersResponse> Handle(
        GetOrdersRequest request,
        CancellationToken cancellationToken)
    {
        Expression<Func<Order, bool>> filter = o => true;

        if (request.CustomerId.HasValue)
        {
            filter = filter.And(o => o.CustomerId.Equals(request.CustomerId));
        }
        
        if (request.Status.HasValue)
        {
            filter = filter.And(o => o.Status.Equals(request.Status));
        }
        
        var orders = await _orderRepository.GetPage(
            request.PageNumber, 
            request.PageSize,
            filter: filter,
            sortInfo: request.SortInfo,
            cancellationToken: cancellationToken);

        return new GetOrdersResponse
        {
            Orders = orders
        };
    }
}