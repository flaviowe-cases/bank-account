using Bank.Commons.Applications.Serializers;
using Bank.Transactions.Application.Gateways;
using Bank.Transactions.Domain.Entities;
using Confluent.Kafka;
using Microsoft.Extensions.Logging;

namespace Bank.Transactions.Infrastructure.Gateways.KafkaBroker;

public class TransactionProducer(
    ILogger<TransactionProducer>  logger,
    TopicNames topicNames,
    IJsonSerializer jsonSerializer,
    IProducer<string, string> producer) : ITransactionProducer
{
    private readonly ILogger<TransactionProducer> _logger = logger;
    private readonly TopicNames _topicNames = topicNames;
    private readonly IJsonSerializer _jsonSerializer = jsonSerializer;
    private readonly IProducer<string, string> _producer = producer;

    public Task ExecuteTransactionAsync(Transaction transaction)
    {
        return ExecuteTransactionAsync(new TopicEnvelop<Transaction>()
        {
            Id = Guid.NewGuid(),
            Message = transaction,
        });
    }

    private async Task ExecuteTransactionAsync(TopicEnvelop<Transaction> envelop)
    {
        var key = envelop.Message.Id.ToString();
        var value = _jsonSerializer.Serialize(envelop);
        var message = new Message<string, string>
        {
            Key = key,
            Value = value,
        };

        await _producer.ProduceAsync(_topicNames.ExecuteTransaction, message);
    }
}