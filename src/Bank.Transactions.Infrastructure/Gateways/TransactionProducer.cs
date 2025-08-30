using Bank.Transactions.Application.Gateways;
using Bank.Transactions.Domain.Entities;
using Confluent.Kafka;

namespace Bank.Transactions.Infrastructure.Gateways;

public class TransactionProducer(
    IProducer<Guid, TransactionMessage> producer) : ITransactionProducer
{
    private const string Topic = "execute-transaction";

    private readonly IProducer<Guid, TransactionMessage> _producer = producer;

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
        var message = new Message<Guid, TransactionMessage>
        {
            Key = transactionMessage.Transaction.Id,
            Value = transactionMessage,
        };

        await _producer.ProduceAsync(Topic, message);
    }
}