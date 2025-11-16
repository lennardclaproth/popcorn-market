using PopcornMarket.Messaging.Contracts.V1.Events;

namespace PopcornMarket.Messaging.Contracts.V1.Constants;

public static class PayloadMap
{
    public static readonly Dictionary<string, Type> Map = new() {
        { TopicConstants.CompanyCreated, typeof(CompanyCreatedPayload) },
        { TopicConstants.CompanyListed, typeof(CompanyListedPayload) },
        { TopicConstants.OrderFulfilled, typeof(OrderFulfilledPayload)},
        { TopicConstants.OrderCancelled, typeof(OrderCancelledPayload)},
        { TopicConstants.OrderPartiallyCancelled, typeof(OrderPartiallyCancelledPayload)},
        { TopicConstants.OrderPartiallyFilled, typeof(OrderPartiallyFilledPayload) },
    };
}
