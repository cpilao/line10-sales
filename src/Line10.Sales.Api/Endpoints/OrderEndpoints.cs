using Line10.Sales.Application.Commands;
using Line10.Sales.Application.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Line10.Sales.Api.Endpoints;

public static class OrderEndpoints
{
    public static IEndpointRouteBuilder AddOrderEndpoints(this IEndpointRouteBuilder endpointRouteBuilder)
    {
        endpointRouteBuilder.MapPost("/orders/{id:guid}/products", async (
                [FromServices] IMediator mediator,
                [FromRoute] Guid id,
                [FromBody] AddOrderProductRequest request) =>
            {
                var response = await mediator.Send(request with {OrderId = id});
                return response.IsSuccess ? Results.NoContent() : Results.BadRequest(response.Errors);
            })
            .WithName("AddOrderProduct");
            
        endpointRouteBuilder.MapDelete("/orders/{id:guid}/products", async (
                [FromServices] IMediator mediator,
                [FromRoute] Guid id,
                [FromBody] RemoveOrderProductRequest request) =>
            {
                var response = await mediator.Send(request with {OrderId = id});
                return response.IsSuccess ? Results.NoContent() : Results.BadRequest(response.Errors);
            })
            .WithName("RemoveOrderProduct");
        
        endpointRouteBuilder.MapGet("/orders/{id:guid}/products", async (
                [FromServices] IMediator mediator,
                [FromRoute] Guid id) =>
            {
                var response = await mediator.Send(new GetOrderProductsRequest {OrderId = id});
                return response.IsSuccess ? Results.Json(response.OrderProducts) : Results.BadRequest(response.Errors);
            })
            .WithName("GetOrderProducts")
            
            .WithOpenApi();
        endpointRouteBuilder.MapPost("/orders", async (
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
        
        endpointRouteBuilder.MapPost("/orders/{id:guid}/process", async (
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
        
        endpointRouteBuilder.MapPost("/orders/{id:guid}/ship", async (
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
        
        endpointRouteBuilder.MapPost("/orders/{id:guid}/delivery", async (
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
        
        endpointRouteBuilder.MapPost("/orders/{id:guid}/cancel", async (
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

        endpointRouteBuilder.MapGet("/orders/{id:guid}", async (
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
        
        endpointRouteBuilder.MapDelete("/orders/{id:guid}", async (
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