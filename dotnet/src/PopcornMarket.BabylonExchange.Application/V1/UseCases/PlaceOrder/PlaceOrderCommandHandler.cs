using AutoMapper;
using MediatR;
using PopcornMarket.BabylonExchange.Domain.Abstractions;
using PopcornMarket.BabylonExchange.Domain.Abstractions.Repositories;
using PopcornMarket.BabylonExchange.Domain.Entities;
using PopcornMarket.BabylonExchange.Domain.Enums;
using PopcornMarket.BabylonExchange.Domain.Errors;
using PopcornMarket.SharedKernel.Abstractions;
using PopcornMarket.SharedKernel.CQRS;
using PopcornMarket.SharedKernel.Extensions;
using PopcornMarket.SharedKernel.ResultPattern;

namespace PopcornMarket.BabylonExchange.Application.V1.UseCases.PlaceOrder;

public class PlaceOrderCommandHandler : ICommandHandler<PlaceOrderCommand, string>
{
    private readonly IOrderBookRepository _orderBookRepository;
    private readonly IDomainEventQueue _eventQueue;
    private readonly IMediator _mediator;
    private readonly IMapper _mapper;
    private readonly IUnitOfWork _unitOfWork;

    public PlaceOrderCommandHandler(IOrderBookRepository orderBookRepository, IMapper mapper, IMediator mediator, IUnitOfWork unitOfWork, IDomainEventQueue eventQueue)
    {
        _orderBookRepository = orderBookRepository;
        _mapper = mapper;
        _mediator = mediator;
        _unitOfWork = unitOfWork;
        _eventQueue = eventQueue;
    }

    public async Task<Result<string>> Handle(PlaceOrderCommand request, CancellationToken cancellationToken)
    {
        var orderBook = await _orderBookRepository.GetByStockSymbol(request.StockSymbol);
        if(orderBook == null) return Result<string>.Failure(OrderBookErrors.OrderBookNotFound);

        var orderType = _mapper.Map<OrderType>(request.Type);
        var orderSide = _mapper.Map<OrderSide>(request.Side);

        var order = Order.Create(request.StockSymbol, request.TraderId, request.Price, request.Quantity, orderType, orderBook, orderSide);
        orderBook.PlaceOrder(order);

        await _orderBookRepository.UpdateEntity(orderBook);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        await _mediator.DispatchDomainEvents(orderBook, cancellationToken);

        return Result<string>.Success(order.OrderId);
    }
}
