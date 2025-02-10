using Line10.Sales.Application.Commands;
using Line10.Sales.Core;
using Line10.Sales.Domain.Persistence;
using MediatR;

namespace Line10.Sales.Application.Handlers;

public class UpdateCustomerCommandHandler: IRequestHandler<UpdateCustomerRequest, VoidResponse>
{
    private readonly ICustomerRepository _customerRepository;

    public UpdateCustomerCommandHandler(ICustomerRepository customerRepository)
    {
        _customerRepository = customerRepository;
    }
    
    public async Task<VoidResponse> Handle(
        UpdateCustomerRequest request,
        CancellationToken cancellationToken)
    {
        var customer = await _customerRepository.GetByIdAsync(request.CustomerId, cancellationToken);

        if (customer == null)
        {
            return new VoidResponse
            {
                Errors = [new Error("NotFound")]
            };
        }

        customer.UpdateName(request.FirstName, request.LastName);
        customer.UpdateEmail(request.Email);
        if (!string.IsNullOrEmpty(request.Phone))
        {
            customer.UpdatePhone(request.Phone);
        }
        
        await _customerRepository.UpdateAsync(customer, cancellationToken);
        return VoidResponse.Success;
    }
}