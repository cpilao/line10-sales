using Line10.Sales.Application.Queries;
using Line10.Sales.Core;
using Line10.Sales.Domain.Persistence;
using MediatR;

namespace Line10.Sales.Application.Handlers;

public class GetCustomerQueryHandler: IRequestHandler<GetCustomerRequest, GetCustomerResponse>
{
    private readonly ICustomerRepository _customerRepository;

    public GetCustomerQueryHandler(ICustomerRepository customerRepository)
    {
        _customerRepository = customerRepository;
    }
    
    public async Task<GetCustomerResponse> Handle(
        GetCustomerRequest request,
        CancellationToken cancellationToken)
    {
        var customer = await _customerRepository.GetByIdAsync(request.CustomerId, cancellationToken);

        if (customer == null)
        {
            return new GetCustomerResponse
            {
                Errors = [new Error("CustomerNotFound")]
            };
        }
        
        return new GetCustomerResponse
        {
            Customer = customer
        };
    }
}