using MediatR;

namespace Line10.Sales.Application.Queries;

public sealed record GetCustomersRequest : IRequest<GetCustomersResponse>
{
    public int PageNumber { get; init; }
    public int PageSize { get; init; }
}