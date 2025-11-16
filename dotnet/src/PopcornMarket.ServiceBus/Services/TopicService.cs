using Confluent.Kafka;
using Microsoft.Extensions.Configuration;

namespace PopcornMarket.ServiceBus.Services;

internal sealed class TopicService
{
    private readonly IAdminClient _adminClient;
    private readonly List<string> _topics = new();
    private readonly List<string> _activeTopics = new();
    private readonly List<string> _brokenTopics = new();
    private readonly Dictionary<string, Type> _payloadMap = new();

    public TopicService(IConfiguration configuration)
    {
        var config = configuration.GetSection("Messaging:Kafka");
        var bootstrapServers = config["BootstrapServers"];
        _adminClient = new AdminClientBuilder(new AdminClientConfig
        {
            BootstrapServers = bootstrapServers
        }).Build();
    }

    public void AddTopics(List<string> topics)
    {
        _topics.AddRange(topics);
    }

    public void AddPayloadMap(Dictionary<string, Type> payloadMap)
    {
        foreach (var mapping in payloadMap)
        {
            _payloadMap.Add(mapping.Key, mapping.Value);
        }
    }

    public Type GetPayloadTypeByTopic(string topic)
    {
        return _payloadMap[topic];
    }

    public IReadOnlyList<string> AllTopics => _topics.AsReadOnly();
    public IReadOnlyList<string> ActiveTopics => _activeTopics.AsReadOnly();
    public IReadOnlyList<string> BrokenTopics => _brokenTopics.AsReadOnly();

    public void RefreshTopicStatus()
    {
        _activeTopics.Clear();
        _brokenTopics.Clear();

        var metadata = _adminClient.GetMetadata(TimeSpan.FromSeconds(5));

        var existingTopics = metadata
            .Topics
            .Where(t => !t.Error.IsError)
            .Select(t => t.Topic)
            .ToHashSet();

        foreach (var topic in _topics)
        {
            if (existingTopics.Contains(topic))
            {
                _activeTopics.Add(topic);
            }
            else
            {
                _brokenTopics.Add(topic);
            }
        }
    }

    public bool IsTopicAvailable(string topic)
    {
        RefreshTopicStatus();
        return _activeTopics.Contains(topic);
    }

    public bool AreAllTopicsAvailable()
    {
        RefreshTopicStatus();
        return _brokenTopics.Count == 0;
    }
}
