using MediatR;

namespace Line10.Sales.Application.Commands;

public sealed record DeleteCustomerRequest : IRequest<VoidResponse>
{
    public Guid CustomerId { get; init; }
}