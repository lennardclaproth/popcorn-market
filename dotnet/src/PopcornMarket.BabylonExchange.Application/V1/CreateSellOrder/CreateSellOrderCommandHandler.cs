using PopcornMarket.BabylonExchange.Domain.Abstractions.Repositories;
using PopcornMarket.BabylonExchange.Domain.Enums;
using PopcornMarket.BabylonExchange.Domain.Errors;
using PopcornMarket.SharedKernel.CQRS;
using PopcornMarket.SharedKernel.ResultPattern;

namespace PopcornMarket.BabylonExchange.Application.V1.CreateSellOrder;

internal sealed class CreateSellOrderCommandHandler : ICommandHandler<CreateSellOrderCommand>
{
    private readonly IOrderBookRepository _orderBookRepository;

    public CreateSellOrderCommandHandler(IOrderBookRepository orderBookRepository)
    {
        _orderBookRepository = orderBookRepository;
    }

    public async Task<Result> Handle(CreateSellOrderCommand request, CancellationToken cancellationToken)
    {
        var orderBook = await _orderBookRepository.GetByTicker(request.Ticker);
        if(orderBook == null) return Result.Failure(OrderBookErrors.OrderBookNotFound);
        
        orderBook.AddSellOrder(request.TraderId, request.Price, request.Quantity, OrderType.MarketOrder);
        
        await _orderBookRepository.UpdateEntity(orderBook);
        return Result.Success();
    }
}
