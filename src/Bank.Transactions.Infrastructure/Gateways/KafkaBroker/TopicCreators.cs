using Confluent.Kafka;
using Confluent.Kafka.Admin;
using Microsoft.Extensions.Logging;

namespace Bank.Transactions.Infrastructure.Gateways.KafkaBroker;

public class TopicCreators(
    ILogger<TopicCreators> logger,
    TopicNames topicNames,
    IAdminClient adminClient) : ITopicCreators
{
    private readonly ILogger<TopicCreators> _logger = logger;
    private readonly TopicNames _topicNames = topicNames;
    private readonly IAdminClient _adminClient = adminClient;

    public async Task CreateTopicsAsync()
    {
        var topicSpecifications = GetTopics(); 
        try
        {
            await _adminClient.CreateTopicsAsync(topicSpecifications);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Failed to create topics");
        }
    }

    private IEnumerable<TopicSpecification> GetTopics() =>
        _topicNames.AllTopics
            .Select(name => new TopicSpecification()
            {
                Name = name,
                NumPartitions = 1,
                ReplicationFactor = 1,
            });
}