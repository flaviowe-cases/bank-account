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
        catch (CreateTopicsException e)
        {
            foreach (var result in e.Results)
            {
                switch (result.Error.Code)
                {
                    case ErrorCode.NoError:
                        _logger.LogInformation("Topic created {TopicName}", result.Topic);
                        break;
                    
                    case ErrorCode.TopicAlreadyExists:
                        _logger.LogInformation("Topic already exists, skipping creation topic: {TopicName}",
                            result.Topic);
                        break;
                    
                    default:
                        _logger.LogError("Error creating topic {Reason} {TopicName}",
                            result.Error.Reason, result.Topic);
                        break;
                }
            }
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