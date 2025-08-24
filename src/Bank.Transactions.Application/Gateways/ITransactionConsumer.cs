using Bank.Transactions.Domain.Entities;

namespace Bank.Transactions.Application.Gateways;

public interface ITransactionConsumer
{
    Task SubscribeAsync(CancellationToken cancellationToken);
    void Unsubscribe();
    Func<Transaction, Task>? OnReceiveAsync {get; set;}
}