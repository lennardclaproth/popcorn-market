using PopcornMarket.BabylonExchange.Domain.Abstractions.Repositories;
using PopcornMarket.BabylonExchange.Domain.Enums;
using PopcornMarket.BabylonExchange.Domain.Errors;
using PopcornMarket.SharedKernel.CQRS;
using PopcornMarket.SharedKernel.ResultPattern;

namespace PopcornMarket.BabylonExchange.Application.V1.CreateBuyOrder;

public class CreateBuyOrderCommandHandler : ICommandHandler<CreateBuyOrderCommand>
{
    private readonly IOrderBookRepository _orderBookRepository;

    public CreateBuyOrderCommandHandler(IOrderBookRepository orderBookRepository)
    {
        _orderBookRepository = orderBookRepository;
    }

    public async Task<Result> Handle(CreateBuyOrderCommand request, CancellationToken cancellationToken)
    {
        var orderBook = await _orderBookRepository.GetByTicker(request.Ticker);
        if(orderBook == null) return Result.Failure(OrderBookErrors.OrderBookNotFound);
        
        orderBook.AddBuyOrder(request.TraderId, request.Price, request.Quantity, OrderType.MarketOrder);
        
        await _orderBookRepository.UpdateEntity(orderBook);
        return Result.Success();
    }
}
