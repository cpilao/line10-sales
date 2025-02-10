using Line10.Sales.Application.Commands;
using Line10.Sales.Domain.Entities;
using Line10.Sales.Domain.Persistence;
using MediatR;

namespace Line10.Sales.Application.Handlers;

public class CreateCustomerCommandHandler: IRequestHandler<CreateCustomerRequest, CreateCustomerResponse>
{
    private readonly ICustomerRepository _customerRepository;

    public CreateCustomerCommandHandler(ICustomerRepository customerRepository)
    {
        _customerRepository = customerRepository;
    }
    
    public async Task<CreateCustomerResponse> Handle(
        CreateCustomerRequest request,
        CancellationToken cancellationToken)
    {
        var result = Customer.Create(
            request.FirstName,
            request.LastName,
            request.Email,
            request.Phone);

        if (result.HasErrors)
        {
            return new CreateCustomerResponse
            {
                Errors = result.Errors
            };
        }
        
        await _customerRepository.AddAsync(result.Content!, cancellationToken);
        return new CreateCustomerResponse
        {
            CustomerId = result.Content!.Id
        };
    }
}