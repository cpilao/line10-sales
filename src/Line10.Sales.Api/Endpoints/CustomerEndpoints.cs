using Line10.Sales.Application.Commands;
using Line10.Sales.Application.Queries;
using Line10.Sales.Domain.Entities;
using Line10.Sales.Domain.Extensions;
using Line10.Sales.Domain.Persistence;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Line10.Sales.Api.Endpoints;

public static class CustomerEndpoints
{
    public static IEndpointRouteBuilder AddCustomerEndpoints(this IEndpointRouteBuilder endpointRouteBuilder)
    {
        var customersApi = endpointRouteBuilder
            .MapGroup("/customers")
            .WithTags("Customers");
        
        customersApi.MapGet(string.Empty, async (
                [FromServices] IMediator mediator,
                [FromQuery] string? orderBy,
                [FromQuery] SortOrder? order,
                [FromQuery] string? firstName,
                [FromQuery] string? lastName,
                [FromQuery] int pageNumber = 1, 
                [FromQuery] int pageSize = 10) =>
            {
                var response = await mediator.Send(new GetCustomersRequest
                {
                    SortInfo = orderBy.GetSortInfo<Customer>(order),
                    FirstName = firstName,
                    LastName = lastName,
                    PageNumber = pageNumber,
                    PageSize = pageSize
                });
                return response.IsSuccess ? 
                    Results.Json(new {response.Customers}) : 
                    Results.BadRequest(response.Errors);
            })
            .WithName("GetCustomers")
            .WithOpenApi();
        
        customersApi.MapPost(string.Empty, async (
                [FromServices] IMediator mediator, 
                [FromBody] CreateCustomerRequest request) =>
            {
                var response = await mediator.Send(request);
                return response.IsSuccess ? 
                    Results.Json(new {response.CustomerId}) : 
                    Results.BadRequest(response.Errors);
            })
            .WithName("CreateCustomer")
            .WithOpenApi();
        
        customersApi.MapPut("/{id:guid}", async (
                [FromServices] IMediator mediator,
                [FromRoute] Guid id,
                [FromBody] UpdateCustomerRequest request) =>
            {
                var response = await mediator.Send(request with {CustomerId = id});
                return response.IsSuccess ? 
                    Results.NoContent() : 
                    Results.BadRequest(response.Errors);
            })
            .WithName("UpdateCustomer")
            .WithOpenApi();

        customersApi.MapGet("/{id:guid}", async (
                [FromServices] IMediator mediator, 
                [FromRoute] Guid id) =>
            {
                var response = await mediator.Send(new GetCustomerRequest
                {
                    CustomerId = id
                });
                return response.IsSuccess ? 
                    Results.Json(new
                    {
                        response.Customer?.FirstName,
                        response.Customer?.LastName,
                        response.Customer?.Email,
                        response.Customer?.Phone
                    }) : 
                    Results.BadRequest(response.Errors);
            })
            .WithName("GetCustomerById")
            .WithOpenApi();
        
        customersApi.MapDelete("/{id:guid}", async (
                [FromServices] IMediator mediator, 
                [FromRoute] Guid id) =>
            {
                var response = await mediator.Send(new DeleteCustomerRequest()
                {
                    CustomerId = id
                });
                return response.IsSuccess ? 
                    Results.NoContent() : 
                    Results.BadRequest(response.Errors);
            })
            .WithName("DeleteCustomer")
            .WithOpenApi();

        return endpointRouteBuilder;
    }
}