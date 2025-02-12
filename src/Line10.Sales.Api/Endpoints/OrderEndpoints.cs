using Line10.Sales.Application.Commands;
using Line10.Sales.Application.Queries;
using Line10.Sales.Domain.Entities;
using Line10.Sales.Domain.Extensions;
using Line10.Sales.Domain.Persistence;
using MediatR;
using Microsoft.AspNetCore.Mvc;

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
                [FromQuery] int pageNumber = 1, 
                [FromQuery] int pageSize = 10) =>
            {
                var response = await mediator.Send(new GetOrdersRequest()
                {
                    SortInfo = orderBy.GetSortInfo<Order>(order),
                    CustomerId = customerId,
                    Status = status,
                    PageNumber = pageNumber,
                    PageSize = pageSize
                });
                return response.IsSuccess ? 
                    Results.Json(new {response.Orders}) : 
                    Results.BadRequest(response.Errors);
            })
            .WithName("GetOrders")
            .WithOpenApi();
        
        ordersApi.MapPost("/{id:guid}/products", async (
                [FromServices] IMediator mediator,
                [FromRoute] Guid id,
                [FromBody] AddOrderProductRequest request) =>
            {
                var response = await mediator.Send(request with {OrderId = id});
                return response.IsSuccess ? Results.NoContent() : Results.BadRequest(response.Errors);
            })
            .WithName("AddOrderProduct");
            
        ordersApi.MapDelete("/{id:guid}/products", async (
                [FromServices] IMediator mediator,
                [FromRoute] Guid id,
                [FromBody] RemoveOrderProductRequest request) =>
            {
                var response = await mediator.Send(request with {OrderId = id});
                return response.IsSuccess ? Results.NoContent() : Results.BadRequest(response.Errors);
            })
            .WithName("RemoveOrderProduct");
        
        ordersApi.MapGet("/{id:guid}/products", async (
                [FromServices] IMediator mediator,
                [FromRoute] Guid id) =>
            {
                var response = await mediator.Send(new GetOrderProductsRequest {OrderId = id});
                return response.IsSuccess ? Results.Json(response.OrderProducts) : Results.BadRequest(response.Errors);
            })
            .WithName("GetOrderProducts")
            .WithOpenApi();
        
        ordersApi.MapPost(string.Empty, async (
                [FromServices] IMediator mediator, 
                [FromBody] CreateOrderRequest request) =>
            {
                var response = await mediator.Send(request);
                return response.IsSuccess ? 
                    Results.Json(new {response.OrderId}) : 
                    Results.BadRequest(response.Errors);
            })
            .WithName("CreateOrder")
            .WithOpenApi();
        
        ordersApi.MapPost("/{id:guid}/process", async (
                [FromServices] IMediator mediator,
                [FromRoute] Guid id) =>
            {
                var response = await mediator.Send(new ProcessOrderRequest {OrderId = id});
                return response.IsSuccess ? 
                    Results.NoContent() : 
                    Results.BadRequest(response.Errors);
            })
            .WithName("ProcessOrder")
            .WithOpenApi();
        
        ordersApi.MapPost("/{id:guid}/ship", async (
                [FromServices] IMediator mediator,
                [FromRoute] Guid id) =>
            {
                var response = await mediator.Send(new ShipOrderRequest {OrderId = id});
                return response.IsSuccess ? 
                    Results.NoContent() : 
                    Results.BadRequest(response.Errors);
            })
            .WithName("ShipOrder")
            .WithOpenApi();
        
        ordersApi.MapPost("/{id:guid}/delivery", async (
                [FromServices] IMediator mediator,
                [FromRoute] Guid id) =>
            {
                var response = await mediator.Send(new DeliveryOrderRequest {OrderId = id});
                return response.IsSuccess ? 
                    Results.NoContent() : 
                    Results.BadRequest(response.Errors);
            })
            .WithName("DeliveryOrder")
            .WithOpenApi();
        
        ordersApi.MapPost("/{id:guid}/cancel", async (
                [FromServices] IMediator mediator,
                [FromRoute] Guid id) =>
            {
                var response = await mediator.Send(new CancelOrderRequest {OrderId = id});
                return response.IsSuccess ? 
                    Results.NoContent() : 
                    Results.BadRequest(response.Errors);
            })
            .WithName("CancelOrder")
            .WithOpenApi();

        ordersApi.MapGet("/{id:guid}", async (
                [FromServices] IMediator mediator, 
                [FromRoute] Guid id) =>
            {
                var response = await mediator.Send(new GetOrderRequest
                {
                    OrderId = id
                });
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
                [FromRoute] Guid id) =>
            {
                var response = await mediator.Send(new DeleteOrderRequest
                {
                    OrderId = id
                });
                return response.IsSuccess ? 
                    Results.NoContent() : 
                    Results.BadRequest(response.Errors);
            })
            .WithName("DeleteOrder")
            .WithOpenApi();

        return endpointRouteBuilder;
    }
}