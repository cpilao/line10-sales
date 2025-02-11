using System.Linq.Expressions;
using Line10.Sales.Application.Queries;
using Line10.Sales.Core.Extensions;
using Line10.Sales.Domain.Entities;
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
        Expression<Func<Customer, bool>> filter = o => true;

        if (!string.IsNullOrEmpty(request.FirstName))
        {
            filter = filter.And(o => o.FirstName.Contains(request.FirstName));
        }
        
        if (!string.IsNullOrEmpty(request.LastName))
        {
            filter = filter.And(o => o.LastName.Contains(request.LastName));
        }
        
        var customers = await _customerRepository.GetPage(
            request.PageNumber, 
            request.PageSize,
            filter: filter,
            sortInfo: request.SortInfo,
            cancellationToken: cancellationToken);

        return new GetCustomersResponse
        {
            Customers = customers
        };
    }
}