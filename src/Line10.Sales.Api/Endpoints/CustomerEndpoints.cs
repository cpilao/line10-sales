using Line10.Sales.Application.Commands;
using Line10.Sales.Application.Queries;
using Line10.Sales.Domain.Entities;
using Line10.Sales.Domain.Extensions;
using Line10.Sales.Domain.Persistence;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;

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
                CancellationToken cancellationToken,
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
                }, cancellationToken);
                return response.IsSuccess ? 
                    Results.Json(new {response.Customers}) : 
                    Results.BadRequest(response.Errors);
            })
            .WithName("GetCustomers")
            .WithOpenApi()
            .CacheOutput(o => o
                .Tag("customers")
                .SetVaryByQuery("*")
                .Expire(TimeSpan.FromMinutes(10)));
        
        customersApi.MapPost(string.Empty, async (
                [FromServices] IMediator mediator,
                [FromServices] IOutputCacheStore cacheStore,
                [FromBody] CreateCustomerRequest request,
                CancellationToken cancellationToken) =>
            {
                var response = await mediator.Send(request, cancellationToken);
                if (response.IsSuccess)
                {
                    // Invalidate the cache customers tag
                    await cacheStore.EvictByTagAsync("customers", cancellationToken);
                    return Results.Json(new {response.CustomerId});
                }
                
                return Results.BadRequest(response.Errors);
            })
            .WithName("CreateCustomer")
            .WithOpenApi();
        
        customersApi.MapPut("/{id:guid}", async (
                [FromServices] IMediator mediator,
                [FromServices] IOutputCacheStore cacheStore,
                [FromRoute] Guid id,
                [FromBody] UpdateCustomerRequest request,
                CancellationToken cancellationToken) =>
            {
                var response = await mediator.Send(request with {CustomerId = id}, cancellationToken);
                if (response.IsSuccess)
                {
                    // Invalidate the cache for the specific customer ID
                    var cacheKey = id.ToString();
                    await cacheStore.EvictByTagAsync(cacheKey, cancellationToken);
                    
                    // Invalidate the cache customers tag
                    await cacheStore.EvictByTagAsync("customers", cancellationToken);
                    return Results.NoContent();
                }

                return Results.BadRequest(response.Errors);
            })
            .WithName("UpdateCustomer")
            .WithOpenApi();

        customersApi.MapGet("/{id:guid}", [OutputCache(PolicyName = "ByIdCachePolicy")] async (
                [FromServices] IMediator mediator,
                [FromRoute] Guid id,
                CancellationToken cancellationToken) =>
            {
                var response = await mediator.Send(new GetCustomerRequest
                {
                    CustomerId = id
                }, cancellationToken);
                return response.IsSuccess
                    ? Results.Json(new
                    {
                        response.Customer?.FirstName,
                        response.Customer?.LastName,
                        response.Customer?.Email,
                        response.Customer?.Phone
                    })
                    : Results.BadRequest(response.Errors);
            })
            .WithName("GetCustomerById")
            .WithOpenApi();
        
        customersApi.MapDelete("/{id:guid}", async (
                [FromServices] IMediator mediator, 
                [FromRoute] Guid id,
                [FromServices] IOutputCacheStore cacheStore,
                CancellationToken cancellationToken) =>
            {
                var response = await mediator.Send(new DeleteCustomerRequest()
                {
                    CustomerId = id
                }, cancellationToken);
                
                if (response.IsSuccess)
                {
                    // Invalidate the cache for the specific customer ID
                    var cacheKey = id.ToString();
                    await cacheStore.EvictByTagAsync(cacheKey, cancellationToken);
                    
                    // Invalidate the cache customers tag
                    await cacheStore.EvictByTagAsync("customers", cancellationToken);
                    return Results.NoContent();
                }
                
                return Results.BadRequest(response.Errors);
            })
            .WithName("DeleteCustomer")
            .WithOpenApi();

        return endpointRouteBuilder;
    }
}