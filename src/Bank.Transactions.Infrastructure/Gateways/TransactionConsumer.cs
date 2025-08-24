using System.Data;
using Bank.Transactions.Application.Gateways;
using Bank.Transactions.Domain.Entities;
using Confluent.Kafka;
using Microsoft.Extensions.Logging;

namespace Bank.Transactions.Infrastructure.Gateways;

public class TransactionConsumer : ITransactionConsumer
{
    public Func<Transaction, Task>? OnReceiveAsync { get; set; }
    private readonly IConsumer<Guid, Transaction> _consumer;
    private readonly ILogger<ITransactionConsumer> _logger;
    
    public TransactionConsumer(
        ILogger<ITransactionConsumer> logger,
        BankTransactionConfigure configure)
    {
        _logger = logger;

        var consumerConfig = new ConsumerConfig
        {
            BootstrapServers = configure.MessageQueueHost,
            AutoOffsetReset = AutoOffsetReset.Earliest
        };

        _consumer = new ConsumerBuilder<Guid, Transaction>(consumerConfig)
            .Build();
    }
    
    public async Task SubscribeAsync(CancellationToken cancellationToken)
    {
        if (OnReceiveAsync == null)
            throw new NoNullAllowedException("OnReceiveAsync is not set");

        _consumer.Subscribe("execute-transaction");

        while (!cancellationToken.IsCancellationRequested)
        {
            try
            {
                var result = _consumer.Consume(cancellationToken);
                await OnReceiveAsync(result.Message.Value);
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