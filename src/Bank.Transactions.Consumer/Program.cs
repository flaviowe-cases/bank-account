using Bank.Transactions.Consumer.Extensions;
using Bank.Transactions.Consumer.Subscribers;
using Bank.Transactions.Infrastructure;
using Bank.Transactions.Infrastructure.Extensions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Bank.Transactions.Consumer;

public static class Program
{
    public static async Task Main(string[] args)
    {
        var host = CreateHostBuilder(args).Build();
        await host.RunAsync();
    }
    
    private static IHostBuilder CreateHostBuilder(string[] args) =>
        Host.CreateDefaultBuilder(args)
            .ConfigureServices(ConfigureServices);

    private static void ConfigureServices(HostBuilderContext context, IServiceCollection services)
    {
        var bankAccountBaseAddress = Environment.GetEnvironmentVariable("BANK_ACCOUNT_BASE_ADDRESS");
        var limitAmountTransferVariable = Environment.GetEnvironmentVariable("LIMIT_AMOUNT_TRANSFER");
        var openTelemetryEndpoint = Environment.GetEnvironmentVariable("OTEL_EXPORTER_OTLP_ENDPOINT");
        var openTelemetryProtocol = Environment.GetEnvironmentVariable("OTEL_EXPORTER_OTLP_PROTOCOL");
        var transactionConnectionString = Environment.GetEnvironmentVariable("TRANSACTION_DB_CONNECTION_STRING");
        var databaseName = Environment.GetEnvironmentVariable("TRANSACTION_DATABASE");

        if (!decimal.TryParse(limitAmountTransferVariable, out var limitAmountTransfer))
            throw new ArgumentException(limitAmountTransferVariable);

        if (string.IsNullOrEmpty(bankAccountBaseAddress))
            throw new ArgumentNullException(nameof(bankAccountBaseAddress));

        if (string.IsNullOrEmpty(openTelemetryEndpoint))
            throw new ArgumentNullException(nameof(openTelemetryEndpoint));

        if (string.IsNullOrEmpty(openTelemetryProtocol))
            throw new ArgumentNullException(nameof(openTelemetryProtocol));

        if (string.IsNullOrEmpty(transactionConnectionString))
            throw new ArgumentNullException(nameof(transactionConnectionString));

        if (string.IsNullOrEmpty(databaseName))
            throw new ArgumentNullException(nameof(databaseName));

        services.AddBankTransactions(new BankTransactionConfigure
        {
            BankAccountBaseAddress = bankAccountBaseAddress,
            LimitAmountTransfer = limitAmountTransfer,
            TransactionConnectionString = transactionConnectionString,
            TransactionDatabaseName = databaseName,
            MessageQueueHost = ""
        });

    services.AddBankTransactionsOpenTelemetry(
            "transaction-worker", 
            openTelemetryEndpoint, 
            openTelemetryProtocol);

        services.AddHostedService<TransactionSubscriber>();
    }
}