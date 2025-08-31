using Bank.Commons.Api;
using Bank.Commons.Api.Extensions;
using Bank.Transactions.Infrastructure;
using Bank.Transactions.Infrastructure.Extensions;

namespace Bank.Transactions.Api;

public class Program
{
    public static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        const string serviceName = "transaction-api";
        
        var bankAccountBaseAddress = Environment.GetEnvironmentVariable("BANK_ACCOUNT_BASE_ADDRESS");
        var limitAmountTransferVariable = Environment.GetEnvironmentVariable("LIMIT_AMOUNT_TRANSFER"); 
        var openTelemetryEndpoint = Environment.GetEnvironmentVariable("OTEL_EXPORTER_OTLP_ENDPOINT");
        var openTelemetryProtocol = Environment.GetEnvironmentVariable("OTEL_EXPORTER_OTLP_PROTOCOL");
        var transactionConnectionString = Environment.GetEnvironmentVariable("TRANSACTION_DB_CONNECTION_STRING");
        var databaseName = Environment.GetEnvironmentVariable("TRANSACTION_DATABASE");
        var messageQueueHost = Environment.GetEnvironmentVariable("MESSAGE_QUEUE_HOST");

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
        
        if (string.IsNullOrEmpty(messageQueueHost))
            throw new ArgumentNullException(nameof(messageQueueHost));
        
        var apiConfiguration = new ApiConfiguration()
        {
            Title = "Bank Transactions API",
            Description = "Bank Transactions API provides a methods to handle transactions",
        };

        builder.AddCommonsOpenTelemetry(
            serviceName,
            openTelemetryEndpoint, 
            openTelemetryProtocol,
            messageQueueHost);

        builder.Services.AddControllers();
        builder.Services.AddCommonsApi(apiConfiguration);
        builder.Services.AddBankTransactions(new BankTransactionConfigure
        {
            BankAccountBaseAddress = bankAccountBaseAddress,
            LimitAmountTransfer = limitAmountTransfer,
            TransactionConnectionString = transactionConnectionString,
            TransactionDatabaseName = databaseName,
            MessageQueueHost = messageQueueHost,
            MessageGroupId = serviceName
        });

        var app = builder.Build();

        app.UseCommonsApi();
        app.UseHttpsRedirection();
        app.MapControllers();

        await app.RunAsync();
    }
}