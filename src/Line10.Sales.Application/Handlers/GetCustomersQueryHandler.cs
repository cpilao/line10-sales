using Line10.Sales.Application.Queries;
using Line10.Sales.Domain.Persistence;
using MediatR;

namespace Line10.Sales.Application.Handlers;

public class GetCustomersQueryHandler: IRequestHandler<GetCustomersRequest, GetCustomersResponse>
{
    private readonly ICustomerRepository _customerRepository;

    public GetCustomersQueryHandler(ICustomerRepository customerRepository)
    {
        _customerRepository = customerRepository;
    }
    
    public async Task<GetCustomersResponse> Handle(
        GetCustomersRequest request,
        CancellationToken cancellationToken)
    {
        var customers = await _customerRepository.GetPage(
            request.PageNumber, 
            request.PageSize,
            cancellationToken: cancellationToken);

        return new GetCustomersResponse
        {
            Customers = customers
        };
    }
}