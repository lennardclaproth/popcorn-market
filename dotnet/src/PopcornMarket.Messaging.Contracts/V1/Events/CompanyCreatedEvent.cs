using System.Text.Json.Serialization;
using PopcornMarket.SharedKernel.Messaging;

namespace PopcornMarket.Messaging.Contracts.V1.Events;

public sealed record CompanyCreatedEvent : IEvent
{
    [JsonPropertyName("ticker")]
    public required string Ticker { get; set; }
    [JsonPropertyName("name")]
    public required string Name { get; set; }
    [JsonPropertyName("industry")]
    public required string Industry { get; set; }
    [JsonPropertyName("description")]
    public required string Description { get; set; }
    [JsonPropertyName("headquarters")]
    public required string Headquarters { get; set; }
    [JsonPropertyName("ceo")]
    public required string Ceo { get; set; }
    [JsonPropertyName("founded_year")]
    public required int FoundedYear { get; set; }
    [JsonPropertyName("employees")]
    public required int Employees { get; set; }
}
