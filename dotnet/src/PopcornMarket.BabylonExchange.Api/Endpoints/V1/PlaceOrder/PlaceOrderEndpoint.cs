using Ardalis.GuardClauses;
using MediatR;
using PopcornMarket.BabylonExchange.Api.Abstractions;
using PopcornMarket.BabylonExchange.Api.Extensions;
using PopcornMarket.BabylonExchange.Application.V1.UseCases.PlaceOrder;
using PopcornMarket.BabylonExchange.Contracts.Requests;
using PopcornMarket.BabylonExchange.Contracts.Responses;

namespace PopcornMarket.BabylonExchange.Api.Endpoints.V1.PlaceOrder;

public class PlaceOrderEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("/api/v1/order/",
            async (PlaceOrderRequest req, ISender sender, CancellationToken ct) =>
            {
                var command = new PlaceOrderCommand()
                {
                    StockSymbol = req.StockSymbol,
                    TraderId = req.TraderId,
                    Quantity = req.Quantity,
                    Price = req.Price,
                    Type = req.Type,
                    Side = req.Side,
                };

                var result = await sender.Send(command, ct);

                if (result.IsFailure)
                {
                    return result.ToProblemDetails();
                }

                Guard.Against.Null(result.Value);

                var response = new PlaceOrderResponse
                {
                    OrderId = result.Value
                };

                return Results.Ok(response);
            }).AllowAnonymous()
            .AddValidation<PlaceOrderRequest>();
    }
}
