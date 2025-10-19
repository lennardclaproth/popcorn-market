namespace PopcornMarket.BabylonExchange.Persistence.Entities;
public class OutboxMessage
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public DateTime OccurredOnUtc { get; set; }
    public string Type { get; set; } = default!;
    public string Payload { get; set; } = default!;
    public DateTime? ProcessedOnUtc { get; set; }
}

