using System.Diagnostics;
using Ardalis.GuardClauses;
using Microsoft.Extensions.Logging;
using PopcornMarket.BabylonExchange.Application.Abstractions;
using PopcornMarket.BabylonExchange.Domain.Abstractions;
using PopcornMarket.BabylonExchange.Domain.Abstractions.Repositories;
using PopcornMarket.BabylonExchange.Domain.Events;
using PopcornMarket.Messaging.Contracts.V1.Events;
using PopcornMarket.SharedKernel.Abstractions;

namespace PopcornMarket.BabylonExchange.Application.V1.DomainEventHandlers;
internal sealed class OrderPartiallyFilledHandler : IDomainEventHandler<OrderPartiallyFilled>
{
    private readonly IOrderRepository _orderRepository;
    private readonly IIntegrationEventDispatcher _integrationEventDispatcher;
    private readonly ILogger<OrderPartiallyFilledHandler> _logger;
    private readonly IUnitOfWork _unitOfWork;

    public OrderPartiallyFilledHandler(IOrderRepository orderRepository,
        ILogger<OrderPartiallyFilledHandler> logger,
        IIntegrationEventDispatcher integrationEventDispatcher,
        IUnitOfWork unitOfWork)
    {
        _orderRepository = orderRepository;
        _logger = logger;
        _integrationEventDispatcher = integrationEventDispatcher;
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(OrderPartiallyFilled notification, CancellationToken cancellationToken)
    {
        _logger.LogDebug("Handling OrderPartiallyFilled event for OrderId: {OrderId} to persist changes", notification.Id);
        var startTime = Stopwatch.GetTimestamp();
        var order = await _orderRepository.GetById(notification.Id);
        Guard.Against.Null(order);
        order.PartiallyFulfillOrder(notification.TradePrice, notification.RemainingQuantity, notification.FulfilledAt);
        await _orderRepository.UpdateEntity(order);
        _logger.LogDebug("Order with OrderId: {OrderId} partial fill has been persisted in {ElapsedTimeMs} ms", notification.Id, Stopwatch.GetElapsedTime(startTime).TotalMilliseconds);

        var payload = new OrderPartiallyFilledPayload
        {
            Id = order.Id,
            TradePrice = notification.TradePrice,
            RemainingQuantity = notification.RemainingQuantity,
            FulfilledAt = notification.FulfilledAt
        };
        var integrationEvent = new OrderPartiallyFilledIntegrationEvent(payload);

        await _integrationEventDispatcher.DispatchToOutbox(integrationEvent, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
