using Line10.Sales.Application.Commands;
using Line10.Sales.Application.Queries;
using Line10.Sales.Domain.Entities;
using Line10.Sales.Domain.Extensions;
using Line10.Sales.Domain.Persistence;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;

namespace Line10.Sales.Api.Endpoints;

public static class ProductEndpoints
{
    public static IEndpointRouteBuilder AddProductEndpoints(this IEndpointRouteBuilder endpointRouteBuilder)
    {
        var productsApi = endpointRouteBuilder
            .MapGroup("/products")
            .WithTags("Products");
        
        productsApi.MapGet(string.Empty, async (
                [FromServices] IMediator mediator, 
                [FromQuery] string? orderBy,
                [FromQuery] SortOrder? order,
                [FromQuery] string? name,
                [FromQuery] string? description,
                [FromQuery] string? sku,
                CancellationToken cancellationToken,
                [FromQuery] int pageNumber = 1, 
                [FromQuery] int pageSize = 10) =>
            {
                var response = await mediator.Send(new GetProductsRequest
                {
                    SortInfo = orderBy.GetSortInfo<Product>(order),
                    Name = name,
                    Description = description,
                    Sku = sku,
                    PageNumber = pageNumber,
                    PageSize = pageSize
                }, cancellationToken);
                return response.IsSuccess ? 
                    Results.Json(new {response.Products}) : 
                    Results.BadRequest(response.Errors);
            })
            .WithName("GetProducts")
            .WithOpenApi()
            .CacheOutput(o => o
                .Tag("products")
                .SetVaryByQuery("*")
                .Expire(TimeSpan.FromMinutes(10)));
        
        productsApi.MapPost(string.Empty, async (
                [FromServices] IMediator mediator, 
                [FromBody] CreateProductRequest request,
                CancellationToken cancellationToken) =>
            {
                var response = await mediator.Send(request, cancellationToken);
                return response.IsSuccess ? 
                    Results.Json(new {response.ProductId}) : 
                    Results.BadRequest(response.Errors);
            })
            .WithName("CreateProduct")
            .WithOpenApi();
        
        productsApi.MapPut("/{id:guid}", async (
                [FromServices] IMediator mediator,
                [FromServices] IOutputCacheStore cacheStore,
                [FromRoute] Guid id,
                [FromBody] UpdateProductRequest request,
                CancellationToken cancellationToken) =>
            {
                var response = await mediator.Send(request with {ProductId = id}, cancellationToken);
                if (response.IsSuccess)
                {
                    // Invalidate the cache for the specific product ID
                    var cacheKey = id.ToString();
                    await cacheStore.EvictByTagAsync(cacheKey, cancellationToken);
                    
                    // Invalidate the cache products tag
                    await cacheStore.EvictByTagAsync("products", cancellationToken);
                    return Results.NoContent();
                }
                return Results.BadRequest(response.Errors);
            })
            .WithName("UpdateProduct")
            .WithOpenApi();

        productsApi.MapGet("/{id:guid}", [OutputCache(PolicyName = "ByIdCachePolicy")] async (
                [FromServices] IMediator mediator, 
                [FromRoute] Guid id,
                CancellationToken cancellationToken) =>
            {
                var response = await mediator.Send(new GetProductRequest
                {
                    ProductId = id
                }, cancellationToken);
                return response.IsSuccess ? 
                    Results.Json(new
                    {
                        response.Product?.Name,
                        response.Product?.Description,
                        response.Product?.Sku
                    }) : 
                    Results.BadRequest(response.Errors);
            })
            .WithName("GetProductById")
            .WithOpenApi();
        
        productsApi.MapDelete("/{id:guid}", async (
                [FromServices] IMediator mediator,
                [FromServices] IOutputCacheStore cacheStore,
                [FromRoute] Guid id,
                CancellationToken cancellationToken) =>
            {
                var response = await mediator.Send(new DeleteProductRequest
                {
                    ProductId = id
                }, cancellationToken);
                if (response.IsSuccess)
                {
                    // Invalidate the cache for the specific product ID
                    var cacheKey = id.ToString();
                    await cacheStore.EvictByTagAsync(cacheKey, cancellationToken);
                    
                    // Invalidate the cache products tag
                    await cacheStore.EvictByTagAsync("products", cancellationToken);
                    return Results.NoContent();
                }
                return Results.BadRequest(response.Errors);
            })
            .WithName("DeleteProduct")
            .WithOpenApi();

        return endpointRouteBuilder;
    }
}