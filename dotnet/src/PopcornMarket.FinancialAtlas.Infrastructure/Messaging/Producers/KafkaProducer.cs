using System.Text;
using System.Text.Json;
using Confluent.Kafka;
using Confluent.Kafka.Extensions.Diagnostics;
using Microsoft.Extensions.Configuration;
using PopcornMarket.FinancialAtlas.Application.Abstractions;

namespace PopcornMarket.FinancialAtlas.Infrastructure.Messaging.Producers;

internal sealed class KafkaProducer : IProducer
{
    private readonly IProducer<string, string> _producer;

    public KafkaProducer(IConfiguration configuration)
    {
        var config = configuration.GetSection("Messaging:Kafka");
        var producerConfig = new ProducerConfig { BootstrapServers = config["BootstrapServers"] };
        _producer = new ProducerBuilder<string, string>(producerConfig)
            .BuildWithInstrumentation();
    }
    
    public async Task PublishAsync<T>(string topic, T message, CancellationToken cancellationToken = default) where T : class
    {
        var json = JsonSerializer.Serialize(message);
        var correlationId = Guid.NewGuid().ToString();
        
        // TODO Implement CorrelationId correctly
        var headers = new Headers { { "x-correlation-id", Encoding.UTF8.GetBytes(correlationId) } };
        
        await _producer.ProduceAsync(topic,
            new Message<string, string>
            {
                Key = topic,
                Value = json,
                Headers = headers
            },
            cancellationToken);
    }
}
