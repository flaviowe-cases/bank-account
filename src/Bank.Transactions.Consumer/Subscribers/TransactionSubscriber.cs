using Bank.Transactions.Application.Gateways;
using Bank.Transactions.Application.UseCases.ExecuteTransaction;
using Bank.Transactions.Domain.Entities;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Bank.Transactions.Consumer.Subscribers;

public class TransactionSubscriber(
    ILogger<TransactionSubscriber> logger,
    ITransactionConsumer  transactionConsumer,
    IServiceScopeFactory  serviceScopeFactory) : IHostedService
{
    private readonly ILogger<TransactionSubscriber> _logger = logger;
    private readonly ITransactionConsumer _transactionConsumer = transactionConsumer;
    private readonly IServiceScopeFactory _serviceScopeFactory = serviceScopeFactory;
    
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Transaction subscriber starting...");
        _transactionConsumer.OnReceiveAsync += HandleTransactionAsync;
        await _transactionConsumer.SubscribeAsync(cancellationToken);
    }
    
    public async Task StopAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Transaction subscriber stopping...");
        _transactionConsumer.Unsubscribe();
        _transactionConsumer.OnReceiveAsync -= HandleTransactionAsync;
        await Task.CompletedTask;
    }

    private async Task HandleTransactionAsync(Transaction transaction)
    {
        using var scope = _serviceScopeFactory.CreateScope();
            
        var useCase = scope.ServiceProvider
            .GetRequiredService<IExecuteTransactionUseCase>();

        var input = new ExecuteTransactionInput
        {
            Transaction = transaction
        };

        var output = await useCase.HandleAsync(input);

        if (output.Success)
        {
            var content = output.GetContent();
            
            _logger.LogInformation("Transaction {Id} end with status {Status}",
                content.Transaction.Id, content.Transaction.Status);
        }
        else
        {
            var errorCode = output.Failures?
                .FirstOrDefault()?.Code ?? "UNDEFINED";
                
            _logger.LogWarning("Transaction {Id} end with error code {Code}",
                input.Transaction.Id, errorCode);
        }
    }
}