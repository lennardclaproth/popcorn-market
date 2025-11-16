using System.Text;
using System.Text.Json;
using Confluent.Kafka;
using Confluent.Kafka.Extensions.Diagnostics;
using Microsoft.Extensions.Configuration;
using PopcornMarket.ServiceBus.Abstractions;
using PopcornMarket.ServiceBus.Services;
using PopcornMarket.SharedKernel.Messaging;

namespace PopcornMarket.ServiceBus.Producers;
internal sealed class KafkaProducer : IProducer
{
    private readonly TopicService _topicService;
    private readonly IProducer<string, string> _producer;

    public KafkaProducer(IConfiguration configuration, TopicService topicService)
    {
        _topicService = topicService;
        var config = configuration.GetSection("Messaging:Kafka");
        var producerConfig = new ProducerConfig { BootstrapServers = config["BootstrapServers"] };
        _producer = new ProducerBuilder<string, string>(producerConfig)
            .BuildWithInstrumentation();
    }

    public async Task Produce<T>(string topic, T payload, CancellationToken cancellationToken = default)
    {
        var json = JsonSerializer.Serialize(payload);
        var correlationId = Guid.NewGuid().ToString();

        // TODO Implement CorrelationId correctly
        var headers = new Headers { { "x-correlation-id", Encoding.UTF8.GetBytes(correlationId) } };

        var result = await _producer.ProduceAsync(topic,
            new Message<string, string>
            {
                Key = topic,
                Value = json,
                Headers = headers
            },
            cancellationToken);
    }
}
