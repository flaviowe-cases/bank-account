using Bank.Transactions.Application.Gateways;
using Bank.Transactions.Domain.Entities;
using Confluent.Kafka;

namespace Bank.Transactions.Infrastructure.Gateways;

public class TransactionProducer : ITransactionProducer
{
    private readonly IProducer<Guid, Transaction> _producer;

    public TransactionProducer(
        BankTransactionConfigure configure)
    {
        var producerConfig = new ProducerConfig
            { BootstrapServers = configure.MessageQueueHost };

        _producer = new ProducerBuilder<Guid, Transaction>(producerConfig)
            .Build();
    }

    public async Task ExecuteTransactionAsync(Transaction transaction)
    {
        var message = new Message<Guid, Transaction>
        {
            Key = transaction.Id,
            Value = transaction,
        };

        await _producer.ProduceAsync("execute-transaction", message);
    }
}