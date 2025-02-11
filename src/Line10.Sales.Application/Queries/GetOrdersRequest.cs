using Line10.Sales.Domain.Entities;
using MediatR;

namespace Line10.Sales.Application.Queries;

public sealed record GetOrdersRequest : BaseQueryRequest<Order>, IRequest<GetOrdersResponse>
{
    public Guid? CustomerId { get; init; }
    public OrderStatus? Status { get; init; }
}