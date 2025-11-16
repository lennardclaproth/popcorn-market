using System.Text.Json.Serialization;
using PopcornMarket.Messaging.Contracts.V1.Constants;
using PopcornMarket.SharedKernel.Messaging;

namespace PopcornMarket.Messaging.Contracts.V1.Events;
public record CompanyListedIntegrationEvent(CompanyListedPayload Payload) : IIntegrationEvent<CompanyListedPayload>
{
    public string Topic { get; } = TopicConstants.CompanyListed;
    public CompanyListedPayload Payload { get; } = Payload;
}

public record CompanyListedPayload
{
    [JsonPropertyName("ticker")]
    public string Ticker { get; init; } = null!;
}
