using AutoMapper;
using MediatR;
using PopcornMarket.BabylonExchange.Domain.Abstractions.Repositories;
using PopcornMarket.BabylonExchange.Domain.Entities;
using PopcornMarket.BabylonExchange.Domain.Enums;
using PopcornMarket.BabylonExchange.Domain.Errors;
using PopcornMarket.SharedKernel.CQRS;
using PopcornMarket.SharedKernel.Extensions;
using PopcornMarket.SharedKernel.ResultPattern;

namespace PopcornMarket.BabylonExchange.Application.V1.PlaceOrder;

public class PlaceOrderCommandHandler : ICommandHandler<PlaceOrderCommand>
{
    private readonly IOrderBookRepository _orderBookRepository;
    private readonly IMediator _mediator;
    private readonly IMapper _mapper;

    public PlaceOrderCommandHandler(IOrderBookRepository orderBookRepository, IMapper mapper, IMediator mediator)
    {
        _orderBookRepository = orderBookRepository;
        _mapper = mapper;
        _mediator = mediator;
    }

    public async Task<Result> Handle(PlaceOrderCommand request, CancellationToken cancellationToken)
    {
        var orderBook = await _orderBookRepository.GetByTicker(request.Ticker);
        if(orderBook == null) return Result.Failure(OrderBookErrors.OrderBookNotFound);

        var orderType = _mapper.Map<OrderType>(request.Type);
        var orderSide = _mapper.Map<OrderSide>(request.Side);
        
        var order = Order.Create(request.Ticker, request.TraderId, request.Price, request.Quantity, orderType, orderBook, orderSide );
        orderBook.AddOrder(order);
        await _orderBookRepository.UpdateEntity(orderBook);
        await _mediator.DispatchDomainEventsAsync(orderBook, cancellationToken);
        
        return Result.Success();
    }
}
