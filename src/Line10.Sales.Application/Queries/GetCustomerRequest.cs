using MediatR;

namespace Line10.Sales.Application.Queries;

public sealed record GetCustomerRequest : IRequest<GetCustomerResponse>
{
    public Guid CustomerId { get; init; }
}