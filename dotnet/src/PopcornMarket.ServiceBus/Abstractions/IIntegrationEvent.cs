namespace PopcornMarket.ServiceBus.Abstractions;

public interface IIntegrationEvent<out TPayload>
{
    string Topic { get; }
    TPayload Payload { get; }
}
