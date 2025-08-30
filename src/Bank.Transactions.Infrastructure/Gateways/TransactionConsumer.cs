using System.Data;
using Bank.Transactions.Application.Gateways;
using Bank.Transactions.Application.Models;
using Bank.Transactions.Domain.Entities;
using Confluent.Kafka;
using Microsoft.Extensions.Logging;

namespace Bank.Transactions.Infrastructure.Gateways;

public class TransactionConsumer(
    ILogger<TransactionConsumer> logger,
    IConsumer<Guid, TransactionMessage> consumer) : ITransactionConsumer
{
    private const string Topic = "execute-transaction";
    public Func<Transaction, Task>? OnReceiveAsync { get; set; }
    private readonly ILogger<TransactionConsumer> _logger = logger;
    private readonly IConsumer<Guid, TransactionMessage> _consumer = consumer;
    
    public async Task SubscribeAsync(CancellationToken cancellationToken)
    {
        if (OnReceiveAsync == null)
            throw new NoNullAllowedException("OnReceiveAsync is not set");
        
        _consumer.Subscribe(Topic);

        while (!cancellationToken.IsCancellationRequested)
        {
            try
            {
                var result = _consumer.Consume(cancellationToken);
                var transactionMessage = result.Message;
                var transaction = transactionMessage.Value.Transaction;
                await OnReceiveAsync(transaction);
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