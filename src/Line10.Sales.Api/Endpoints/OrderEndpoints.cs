using Line10.Sales.Application.Commands;
using Line10.Sales.Application.Queries;
using Line10.Sales.Domain.Entities;
using Line10.Sales.Domain.Extensions;
using Line10.Sales.Domain.Persistence;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;

namespace Line10.Sales.Api.Endpoints;

public static class OrderEndpoints
{
    public static IEndpointRouteBuilder AddOrderEndpoints(this IEndpointRouteBuilder endpointRouteBuilder)
    {
        var ordersApi = endpointRouteBuilder
            .MapGroup("/orders")
            .WithTags("Orders");
        
        ordersApi.MapGet(string.Empty, async (
                [FromServices] IMediator mediator,
                [FromQuery] string? orderBy,
                [FromQuery] SortOrder? order,
                [FromQuery] Guid? customerId,
                [FromQuery] OrderStatus? status,
                CancellationToken cancellationToken,
                [FromQuery] int pageNumber = 1, 
                [FromQuery] int pageSize = 10) =>
            {
                var response = await mediator.Send(new GetOrdersRequest
                {
                    SortInfo = orderBy.GetSortInfo<Order>(order),
                    CustomerId = customerId,
                    Status = status,
                    PageNumber = pageNumber,
                    PageSize = pageSize
                }, cancellationToken);
                return response.IsSuccess ? 
                    Results.Json(new {response.Orders}) : 
                    Results.BadRequest(response.Errors);
            })
            .WithName("GetOrders")
            .WithOpenApi()
            .CacheOutput(o => o
                .Tag("orders")
                .SetVaryByQuery("*")
                .Expire(TimeSpan.FromMinutes(10)));

        ordersApi.MapPost("/{id:guid}/products", async (
                [FromServices] IMediator mediator,
                [FromRoute] Guid id,
                [FromBody] AddOrderProductRequest request,
                [FromServices] IOutputCacheStore cacheStore,
                CancellationToken cancellationToken) =>
            {
                var response = await mediator.Send(request with {OrderId = id}, cancellationToken);
                if (response.IsSuccess)
                {
                    // Invalidate the cache orders products
                    var cacheKey = $"/orders/{id}/products";
                    await cacheStore.EvictByTagAsync(cacheKey, cancellationToken);
                    return Results.NoContent();
                }
                return Results.BadRequest(response.Errors);
            })
            .WithName("AddOrderProduct");
            
        ordersApi.MapDelete("/{id:guid}/products", async (
                [FromServices] IMediator mediator,
                [FromServices] IOutputCacheStore cacheStore,
                [FromRoute] Guid id,
                [FromBody] RemoveOrderProductRequest request,
                CancellationToken cancellationToken) =>
            {
                var response = await mediator.Send(request with {OrderId = id}, cancellationToken);
                if (response.IsSuccess)
                {
                    // Invalidate the cache orders products
                    var cacheKey = $"/orders/{id}/products";
                    await cacheStore.EvictByTagAsync(cacheKey, cancellationToken);
                    return Results.NoContent();
                }
                return Results.BadRequest(response.Errors);
            })
            .WithName("RemoveOrderProduct");

        ordersApi.MapGet("/{id:guid}/products", [OutputCache(PolicyName = "ByRequestPathCachePolicy")] async (
                [FromServices] IMediator mediator,
                [FromRoute] Guid id,
                CancellationToken cancellationToken) =>
            {
                var response = await mediator.Send(new GetOrderProductsRequest {OrderId = id}, cancellationToken);
                return response.IsSuccess ? Results.Json(response.OrderProducts) : Results.BadRequest(response.Errors);
            })
            .WithName("GetOrderProducts")
            .WithOpenApi();
        
        ordersApi.MapPost(string.Empty, async (
                [FromServices] IMediator mediator,
                [FromServices] IOutputCacheStore cacheStore,
                [FromBody] CreateOrderRequest request,
                CancellationToken cancellationToken) =>
            {
                var response = await mediator.Send(request, cancellationToken);
                if (response.IsSuccess)
                {
                    // Invalidate the cache customers tag
                    await cacheStore.EvictByTagAsync("customers", cancellationToken);
                    return Results.Json(new {response.OrderId});
                }
                
                return Results.BadRequest(response.Errors);
            })
            .WithName("CreateOrder")
            .WithOpenApi();
        
        ordersApi.MapPost("/{id:guid}/process", async (
                [FromServices] IMediator mediator,
                [FromRoute] Guid id,
                [FromServices] IOutputCacheStore cacheStore,
                CancellationToken cancellationToken) =>
            {
                var response = await mediator.Send(new ProcessOrderRequest {OrderId = id}, cancellationToken);
                if (response.IsSuccess)
                {
                    // Invalidate the cache for the specific order ID
                    var cacheKey = id.ToString();
                    await cacheStore.EvictByTagAsync(cacheKey, cancellationToken);
                    
                    // Invalidate the cache orders tag
                    await cacheStore.EvictByTagAsync("orders", cancellationToken);
                    
                    return Results.NoContent();
                }
                return Results.BadRequest(response.Errors);
            })
            .WithName("ProcessOrder")
            .WithOpenApi();
        
        ordersApi.MapPost("/{id:guid}/ship", async (
                [FromServices] IMediator mediator,
                [FromRoute] Guid id,
                [FromServices] IOutputCacheStore cacheStore,
                CancellationToken cancellationToken) =>
            {
                var response = await mediator.Send(new ShipOrderRequest {OrderId = id}, cancellationToken);
                if (response.IsSuccess)
                {
                    // Invalidate the cache for the specific order ID
                    var cacheKey = id.ToString();
                    await cacheStore.EvictByTagAsync(cacheKey, cancellationToken);
                    
                    // Invalidate the cache orders tag
                    await cacheStore.EvictByTagAsync("orders", cancellationToken);
                    
                    return Results.NoContent();
                }
                return Results.BadRequest(response.Errors);
            })
            .WithName("ShipOrder")
            .WithOpenApi();
        
        ordersApi.MapPost("/{id:guid}/delivery", async (
                [FromServices] IMediator mediator,
                [FromRoute] Guid id,
                [FromServices] IOutputCacheStore cacheStore,
                CancellationToken cancellationToken) =>
            {
                var response = await mediator.Send(new DeliveryOrderRequest {OrderId = id}, cancellationToken);
                if (response.IsSuccess)
                {
                    // Invalidate the cache for the specific order ID
                    var cacheKey = id.ToString();
                    await cacheStore.EvictByTagAsync(cacheKey, cancellationToken);
                    
                    // Invalidate the cache orders tag
                    await cacheStore.EvictByTagAsync("orders", cancellationToken);
                    
                    return Results.NoContent();
                }
                return Results.BadRequest(response.Errors);
            })
            .WithName("DeliveryOrder")
            .WithOpenApi();
        
        ordersApi.MapPost("/{id:guid}/cancel", async (
                [FromServices] IMediator mediator,
                [FromRoute] Guid id,
                [FromServices] IOutputCacheStore cacheStore,
                CancellationToken cancellationToken) =>
            {
                var response = await mediator.Send(new CancelOrderRequest {OrderId = id}, cancellationToken);
                if (response.IsSuccess)
                {
                    // Invalidate the cache for the specific order ID
                    var cacheKey = id.ToString();
                    await cacheStore.EvictByTagAsync(cacheKey, cancellationToken);
                    
                    // Invalidate the cache orders tag
                    await cacheStore.EvictByTagAsync("orders", cancellationToken);
                    
                    return Results.NoContent();
                }
                return Results.BadRequest(response.Errors);
            })
            .WithName("CancelOrder")
            .WithOpenApi();

        ordersApi.MapGet("/{id:guid}", [OutputCache(PolicyName = "ByIdCachePolicy")] async (
                [FromServices] IMediator mediator, 
                [FromRoute] Guid id,
                CancellationToken cancellationToken) =>
            {
                var response = await mediator.Send(new GetOrderRequest
                {
                    OrderId = id
                }, cancellationToken);
                return response.IsSuccess ? 
                    Results.Json(new
                    {
                        OrderId = id,
                        response.CustomerId,
                        response.ProductId,
                        response.Status
                    }) : 
                    Results.BadRequest(response.Errors);
            })
            .WithName("GetOrderById")
            .WithOpenApi();
        
        ordersApi.MapDelete("/{id:guid}", async (
                [FromServices] IMediator mediator, 
                [FromRoute] Guid id,
                [FromServices] IOutputCacheStore cacheStore,
                CancellationToken cancellationToken) =>
            {
                var response = await mediator.Send(new DeleteOrderRequest
                {
                    OrderId = id
                }, cancellationToken);
                if (response.IsSuccess)
                {
                    // Invalidate the cache for the specific order ID
                    var cacheKey = id.ToString();
                    await cacheStore.EvictByTagAsync(cacheKey, cancellationToken);
                    
                    // Invalidate the cache orders tag
                    await cacheStore.EvictByTagAsync("orders", cancellationToken);
                    
                    return Results.NoContent();
                }
                return Results.BadRequest(response.Errors);
            })
            .WithName("DeleteOrder")
            .WithOpenApi();

        return endpointRouteBuilder;
    }
}