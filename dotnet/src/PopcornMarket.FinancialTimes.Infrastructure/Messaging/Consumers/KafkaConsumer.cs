using System.Text.Json;
using Ardalis.GuardClauses;
using Confluent.Kafka;
using Confluent.Kafka.Extensions.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using PopcornMarket.FinancialTimes.Application.Abstractions;
using PopcornMarket.Messaging.Contracts.V1.Constants;
using PopcornMarket.SharedKernel.Messaging;

namespace PopcornMarket.FinancialTimes.Infrastructure.Messaging.Consumers;

internal sealed class KafkaConsumer : IConsumer, IDisposable
{
    private readonly ILogger<KafkaConsumer> _logger;
    private readonly IServiceProvider _serviceProvider;
    private readonly IConfiguration _configuration;
    private readonly IConsumer<string, string> _consumer;
    private static readonly List<string> _topics = [TopicConstants.CompanyCreated];
    
    public KafkaConsumer(ILogger<KafkaConsumer> logger, IServiceProvider serviceProvider, IConfiguration configuration)
    {
        _logger = logger;
        _serviceProvider = serviceProvider;
        _configuration = configuration;
        
        var config = _configuration.GetSection("Messaging:Kafka");

        var consumerConfig = new ConsumerConfig
        {
            BootstrapServers = config["BootstrapServers"],
            GroupId = config["GroupId"],
            AutoOffsetReset = AutoOffsetReset.Earliest
        };
        
        _consumer = new ConsumerBuilder<string, string>(consumerConfig).Build();
    }

    public Task StartConsuming(CancellationToken cancellationToken)
    {
        foreach (var topic in _topics)
        {
            _consumer.Subscribe(topic);
        }
        
        Task.Run(() =>
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                _consumer.ConsumeWithInstrumentation(async (result, ct) =>
                {
                    try
                    {
                        Guard.Against.Null(result, nameof(result));
                        var topic = result.Topic;
                        if (!TopicEventMap.Map.TryGetValue(topic, out var type))
                        {
                            _logger.LogWarning("Unhandled topic: {Topic}", topic);
                            return;
                        }
                
                        var message = JsonSerializer.Deserialize(result.Message.Value, type);

                        using var scope = _serviceProvider.CreateScope();
                        var handlerType = typeof(IEventHandler<>).MakeGenericType(type);
                        dynamic handler = scope.ServiceProvider.GetService(handlerType) ?? throw new InvalidOperationException();
                
                        if (message != null)
                        {
                            await handler.HandleAsync((dynamic) message, ct);
                            _logger.LogInformation("Handled event on topic: {Topic} as {Group}", result.Topic, _configuration.GetSection("Messaging:Kafka:GroupId").Value);
                        }
                    }
                    catch (OperationCanceledException) { }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Error while consuming messages from topic: {Topic} as {Group}. Error: {Exception}", result?.Topic, _configuration.GetSection("Messaging:Kafka:GroupId").Value, ex.Message);
                    }
                }, cancellationToken);
            }
        }, cancellationToken);
        return Task.CompletedTask;
    }

    public void Dispose()
    {
        _consumer.Close();
        _consumer.Dispose();
    }
}
