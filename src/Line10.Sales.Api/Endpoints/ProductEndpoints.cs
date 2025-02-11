using Line10.Sales.Application.Commands;
using Line10.Sales.Application.Queries;
using Line10.Sales.Core;
using Line10.Sales.Domain.Entities;
using Line10.Sales.Domain.Extensions;
using Line10.Sales.Domain.Persistence;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Line10.Sales.Api.Endpoints;

public static class ProductEndpoints
{
    public static IEndpointRouteBuilder AddProductEndpoints(this IEndpointRouteBuilder endpointRouteBuilder)
    {
        endpointRouteBuilder.MapGet("/products", async (
                [FromServices] IMediator mediator, 
                [FromQuery] string? orderBy,
                [FromQuery] SortOrder? order,
                [FromQuery] string? name,
                [FromQuery] string? description,
                [FromQuery] string? sku,
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
                });
                return response.IsSuccess ? 
                    Results.Json(new {response.Products}) : 
                    Results.BadRequest(response.Errors);
            })
            .WithName("GetProducts")
            .WithOpenApi();
        
        endpointRouteBuilder.MapPost("/products", async (
                [FromServices] IMediator mediator, 
                [FromBody] CreateProductRequest request) =>
            {
                var response = await mediator.Send(request);
                return response.IsSuccess ? 
                    Results.Json(new {response.ProductId}) : 
                    Results.BadRequest(response.Errors);
            })
            .WithName("CreateProduct")
            .WithOpenApi();
        
        endpointRouteBuilder.MapPut("/products/{id:guid}", async (
                [FromServices] IMediator mediator,
                [FromRoute] Guid id,
                [FromBody] UpdateProductRequest request) =>
            {
                var response = await mediator.Send(request with {ProductId = id});
                return response.IsSuccess ? 
                    Results.NoContent() : 
                    Results.BadRequest(response.Errors);
            })
            .WithName("UpdateProduct")
            .WithOpenApi();

        endpointRouteBuilder.MapGet("/products/{id:guid}", async (
                [FromServices] IMediator mediator, 
                [FromRoute] Guid id) =>
            {
                var response = await mediator.Send(new GetProductRequest
                {
                    ProductId = id
                });
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
        
        endpointRouteBuilder.MapDelete("/products/{id:guid}", async (
                [FromServices] IMediator mediator, 
                [FromRoute] Guid id) =>
            {
                var response = await mediator.Send(new DeleteProductRequest
                {
                    ProductId = id
                });
                return response.IsSuccess ? 
                    Results.NoContent() : 
                    Results.BadRequest(response.Errors);
            })
            .WithName("DeleteProduct")
            .WithOpenApi();

        return endpointRouteBuilder;
    }
}