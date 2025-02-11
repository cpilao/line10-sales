using Line10.Sales.Application.Commands;
using Line10.Sales.Application.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Line10.Sales.Api.Endpoints;

public static class CustomerEndpoints
{
    public static IEndpointRouteBuilder AddCustomerEndpoints(this IEndpointRouteBuilder endpointRouteBuilder)
    {
        endpointRouteBuilder.MapGet("/customers", async (
                [FromServices] IMediator mediator, 
                [FromQuery] int pageNumber = 1, 
                [FromQuery] int pageSize = 10) =>
            {
                var response = await mediator.Send(new GetCustomersRequest
                {
                    PageNumber = pageNumber,
                    PageSize = pageSize
                });
                return response.IsSuccess ? 
                    Results.Json(new {response.Customers}) : 
                    Results.BadRequest(response.Errors);
            })
            .WithName("GetCustomers")
            .WithOpenApi();
        
        endpointRouteBuilder.MapPost("/customers", async (
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
        
        endpointRouteBuilder.MapPut("/customers/{id:guid}", async (
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

        endpointRouteBuilder.MapGet("/customers/{id:guid}", async (
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
        
        endpointRouteBuilder.MapDelete("/customers/{id:guid}", async (
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