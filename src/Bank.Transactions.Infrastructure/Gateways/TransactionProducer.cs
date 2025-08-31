using Bank.Commons.Applications.Serializers;
using Bank.Transactions.Application.Gateways;
using Bank.Transactions.Domain.Entities;
using Confluent.Kafka;

namespace Bank.Transactions.Infrastructure.Gateways;

public class TransactionProducer(
    IJsonSerializer jsonSerializer,
    IProducer<string, string> producer) : ITransactionProducer
{
    private const string Topic = "execute-transaction";

    private readonly IJsonSerializer _jsonSerializer = jsonSerializer;
    private readonly IProducer<string, string> _producer = producer;

    public Task ExecuteTransactionAsync(Transaction transaction)
    {
        return ExecuteTransactionAsync(new TransactionMessage()
        {
            Id = Guid.NewGuid(),
            Transaction = transaction,
        });
    }

    private async Task ExecuteTransactionAsync(TransactionMessage transactionMessage)
    {
        var key = transactionMessage.Transaction.Id.ToString();
        var value = _jsonSerializer.Serialize(transactionMessage);
        var message = new Message<string, string>
        {
            Key = key,
            Value = value,
        };

        await _producer.ProduceAsync(Topic, message);
    }
}