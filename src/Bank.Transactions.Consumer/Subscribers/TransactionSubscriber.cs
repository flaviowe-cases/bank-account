using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Bank.Transactions.Consumer.Subscribers;

public class TransactionSubscriber(
    ILogger<TransactionSubscriber> logger) : IHostedService
{
    private readonly ILogger<TransactionSubscriber> _logger = logger;

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            _logger.LogInformation("Waiting for transactions...");
            await Task.Delay(TimeSpan.FromSeconds(10), cancellationToken);
        }
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Stopping...");
        await Task.CompletedTask;
    }
}