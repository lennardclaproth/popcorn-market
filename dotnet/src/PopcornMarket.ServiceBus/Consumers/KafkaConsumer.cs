using System.Text.Json;
using Confluent.Kafka;
using Confluent.Kafka.Extensions.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using PopcornMarket.ServiceBus.Abstractions;
using PopcornMarket.ServiceBus.Services;
using PopcornMarket.SharedKernel.Exceptions;

namespace PopcornMarket.ServiceBus.Consumers;

internal sealed class KafkaConsumer : IConsumer, IDisposable
{
    private readonly TopicService _topicService;
    private readonly IServiceProvider _serviceProvider;
    private IConsumer<string, string> _consumer;
    private readonly IConfiguration _configuration;
    private readonly ILogger<KafkaConsumer> _logger;

    public KafkaConsumer(TopicService topicService, IServiceProvider serviceProvider, IConfiguration configuration, ILogger<KafkaConsumer> logger)
    {
        _topicService = topicService;
        _serviceProvider = serviceProvider;
        _configuration = configuration;
        _logger = logger;

        var config = _configuration.GetSection("Messaging:Kafka");

        var consumerConfig = new ConsumerConfig
        {
            BootstrapServers = config["BootstrapServers"],
            GroupId = config["GroupId"],
            AutoOffsetReset = AutoOffsetReset.Earliest
        };

        _consumer = new ConsumerBuilder<string, string>(consumerConfig).Build();
    }

    private void RecreateConsumer()
    {
        var config = _configuration.GetSection("Messaging:Kafka");

        var consumerConfig = new ConsumerConfig
        {
            BootstrapServers = config["BootstrapServers"],
            GroupId = config["GroupId"],
            AutoOffsetReset = AutoOffsetReset.Earliest
        };

        _consumer.Dispose();
        _consumer = new ConsumerBuilder<string, string>(consumerConfig).Build();
    }

    public Task StartConsuming(CancellationToken cancellationToken)
    {
        return Task.Run(async () =>
        {
            var currentSubscribed = new HashSet<string>();

            while (!cancellationToken.IsCancellationRequested)
            {
                // 1. Re-evaluate which topics exist
                _topicService.RefreshTopicStatus();
                var activeTopics = _topicService.ActiveTopics.ToHashSet();

                if (activeTopics.Count == 0)
                {
                    // No topics yet – unsubscribe and chill
                    if (currentSubscribed.Count > 0)
                    {
                        _logger.LogInformation("No active topics anymore. Unsubscribing.");
                        _consumer.Unsubscribe();
                        currentSubscribed.Clear();
                    }

                    _logger.LogInformation("No active topics. Retrying in 5 seconds...");
                    await Task.Delay(TimeSpan.FromSeconds(5), cancellationToken);
                    continue;
                }

                // 2. Subscribed topics changed? Re-subscribe
                if (!activeTopics.SetEquals(currentSubscribed))
                {
                    _logger.LogInformation("Subscribing to topics: {Topics}", activeTopics);
                    _consumer.Subscribe(activeTopics);
                    currentSubscribed = activeTopics;
                }

                try
                {
                    // 3. Consume until something Kafka-ish breaks or we get cancelled
                    await _consumer.ConsumeWithInstrumentation(async (result, ct) =>
                    {
                        try
                        {
                            if (result == null)
                            {
                                throw new RequiredPropertyIsNullException(nameof(result));
                            }

                            var topic = result.Topic;
                            var type = _topicService.GetPayloadTypeByTopic(topic);
                            var message = JsonSerializer.Deserialize(result.Message.Value, type);

                            using var scope = _serviceProvider.CreateScope();
                            var handlerType = typeof(IIntegrationEventHandler<>).MakeGenericType(type);
                            var handler = (IIntegrationEventHandler)scope.ServiceProvider.GetRequiredService(handlerType);

                            if (message != null)
                            {
                                await handler.Handle(message, ct);
                            }
                        }
                        catch (OperationCanceledException) { }
                        catch (Exception ex)
                        {
                            _logger.LogError(ex,
                                "Error while consuming messages from topic: {Topic} as {Group}. Error: {Exception}",
                                result?.Topic,
                                _configuration["Messaging:Kafka:GroupId"],
                                ex.Message);
                        }
                    }, cancellationToken);
                }
                catch (ConsumeException ex) when (
                    ex.Error.Code == ErrorCode.UnknownTopicOrPart ||
                    ex.Error.Code == ErrorCode.TopicAuthorizationFailed)
                {
                    // Topic(s) broken – break out, refresh, and try again next loop
                    _logger.LogWarning(ex, "Topic problem detected, will refresh topic list and resubscribe.");
                    currentSubscribed.Clear();
                    RecreateConsumer();
                }
                catch (OperationCanceledException)
                {
                    break;
                }
            }

            try
            {
                _consumer.Unsubscribe();
                _consumer.Close();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during consumer cleanup: {Message}", ex.Message);
            }
        }, cancellationToken);
    }


    private async Task HandleMessage(ConsumeResult<string, string> message, CancellationToken cancellationToken)
    {
        var topic = message.Topic;
        var payloadType = _topicService.GetPayloadTypeByTopic(topic);

        //var topic = message.Topic;
        //if (!TopicEventMap.Map.TryGetValue(topic, out var type))
        //{
        //    _logger.LogWarning("Unhandled topic: {Topic}", topic);
        //    return;
        //}

        var payload = JsonSerializer.Deserialize(message.Message.Value, payloadType);

        using var scope = _serviceProvider.CreateScope();
        var handlerType = typeof(IIntegrationEventHandler<>).MakeGenericType(payloadType);
        dynamic handler = scope.ServiceProvider.GetService(handlerType) ?? throw new InvalidOperationException();

        if (payload != null)
        {
            await handler.HandleAsync((dynamic)payload, cancellationToken);
            _logger.LogInformation("Handled event on topic: {Topic} as {Group}", message.Topic, _configuration.GetSection("Messaging:Kafka:GroupId").Value);
        }
    }

    public void Dispose()
    {
        _consumer.Close();
        _consumer.Dispose();
    }
}
