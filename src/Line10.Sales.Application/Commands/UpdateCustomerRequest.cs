using MediatR;

namespace Line10.Sales.Application.Commands;

public sealed record UpdateCustomerRequest : IRequest<VoidResponse>
{
    public Guid CustomerId { get; init; }
    public string FirstName { get; init; } = string.Empty;
    public string LastName { get; init; } = string.Empty;
    public string Email { get; init; } = string.Empty;
    public string? Phone { get; init; }
}