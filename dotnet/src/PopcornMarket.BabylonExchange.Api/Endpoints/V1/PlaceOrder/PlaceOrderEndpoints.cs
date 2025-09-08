using MediatR;
using PopcornMarket.BabylonExchange.Api.Abstractions;
using PopcornMarket.BabylonExchange.Api.Extensions;
using PopcornMarket.BabylonExchange.Application.V1.PlaceOrder;
using PopcornMarket.BabylonExchange.Contracts.Requests;

namespace PopcornMarket.BabylonExchange.Api.Endpoints.V1.PlaceOrder;

public class PlaceOrderEndpoints : IEndpoint
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
                return result.IsFailure ? result.ToProblemDetails() : Results.Created();
            }).AllowAnonymous();
    }
}
