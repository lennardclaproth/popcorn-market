using System.Globalization;
using Ardalis.GuardClauses;
using Grpc.Core;
using MediatR;
using PopcornMarket.BabylonExchange.Application.V1.UseCases.PlaceOrder;
using PopcornMarket.BabylonExchange.Contracts;
using OrderSide = PopcornMarket.BabylonExchange.Contracts.Enums.OrderSide;
using OrderType = PopcornMarket.BabylonExchange.Contracts.Enums.OrderType;

namespace PopcornMarket.BabylonExchange.Api.Services;

public sealed class OrderService : Contracts.OrderService.OrderServiceBase
{
    private readonly ISender _sender;

    public OrderService(ISender sender)
    {
        _sender = sender;
    }

    public override async Task<PlaceOrderResponse> PlaceOrder(PlaceOrderRequest request, ServerCallContext context)
    {
        var command = new PlaceOrderCommand()
        {
            StockSymbol = request.StockSymbol,
            TraderId = request.TraderId,
            Quantity = request.Quantity,
            Price = Convert.ToDecimal(request.Price),
            Type = (OrderType) request.Type,
            Side = (OrderSide) request.Side,
        };

        var result = await _sender.Send(command, context.CancellationToken);

        if (result.IsFailure)
        {

            throw new RpcException(new Status(StatusCode.InvalidArgument, "Order placement failed."));
        }

        Guard.Against.Null(result.Value);

        var response = new PlaceOrderResponse
        {
            OrderId = result.Value
        };

        return response;
        //return Results.Ok(response);
    }
}
