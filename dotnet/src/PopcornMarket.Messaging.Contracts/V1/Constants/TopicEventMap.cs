using System.Xml;
using PopcornMarket.Messaging.Contracts.V1.Events;

namespace PopcornMarket.Messaging.Contracts.V1.Constants;

public static class TopicEventMap
{
    public static readonly Dictionary<string, Type> Map = new() {
        { TopicConstants.CompanyCreated, typeof(CompanyCreatedEvent) },
        { TopicConstants.MarketDataPublished, typeof(MarketDataPublishedEvent)}
    };
}
