using Line10.Sales.Application.Commands;
using Line10.Sales.Core;
using Line10.Sales.Domain.Persistence;
using MediatR;

namespace Line10.Sales.Application.Handlers;

public class DeleteCustomerQueryHandler: IRequestHandler<DeleteCustomerRequest, VoidResponse>
{
    private readonly ICustomerRepository _customerRepository;

    public DeleteCustomerQueryHandler(ICustomerRepository customerRepository)
    {
        _customerRepository = customerRepository;
    }
    
    public async Task<VoidResponse> Handle(
        DeleteCustomerRequest request,
        CancellationToken cancellationToken)
    {
        var result = await _customerRepository.DeleteAsync(request.CustomerId, cancellationToken);
        return result
            ? VoidResponse.Success
            : new VoidResponse
            {
                Errors = [new Error("CustomerNotFound")]
            };
    }
}