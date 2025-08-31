using System.Data;
using Bank.Commons.Applications.Serializers;
using Bank.Transactions.Application.Gateways;
using Bank.Transactions.Domain.Entities;
using Confluent.Kafka;
using Microsoft.Extensions.Logging;

namespace Bank.Transactions.Infrastructure.Gateways.KafkaBroker;

public class TransactionConsumer(
    ILogger<TransactionConsumer> logger,
    TopicNames topicNames,
    IJsonSerializer jsonSerializer,
    IConsumer<string, string> consumer) : ITransactionConsumer
{
    public Func<Transaction, Task>? OnReceiveAsync { get; set; }
    private readonly ILogger<TransactionConsumer> _logger = logger;
    private readonly TopicNames _topicNames = topicNames;
    private readonly IJsonSerializer _jsonSerializer = jsonSerializer;
    private readonly IConsumer<string, string> _consumer = consumer;

    public async Task SubscribeAsync(CancellationToken cancellationToken)
    {
        if (OnReceiveAsync == null)
            throw new NoNullAllowedException("OnReceiveAsync is not set");

        _consumer.Subscribe(TopicNames.ExecuteTransaction);

        while (!cancellationToken.IsCancellationRequested)
        {
            try
            {
                var result = _consumer.Consume(cancellationToken);
                var json = result.Message.Value;
                var transactionMessage = _jsonSerializer
                    .Deserialize<TopicEnvelop<Transaction>>(json);
                
                if (transactionMessage is not null)
                    await OnReceiveAsync(transactionMessage.Message);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Consumer transaction error {Message}",
                    e.Message);
            }
        }
    }

    public void Unsubscribe()
        => _consumer.Unsubscribe();
}