using Bank.Commons.Api;
using Bank.Commons.Api.Extensions;
using Bank.Transactions.Infrastructure.Extensions;

namespace Bank.Transactions.Api;

public class Program
{
    public static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        var bankAccountBaseAddress = Environment.GetEnvironmentVariable("BANK_ACCOUNT_BASE_ADDRESS");
        var limitAmountTransferVariable = Environment.GetEnvironmentVariable("LIMIT_AMOUNT_TRANSFER"); 
        var openTelemetryEndpoint = Environment.GetEnvironmentVariable("OTEL_EXPORTER_OTLP_ENDPOINT");
        var openTelemetryProtocol = Environment.GetEnvironmentVariable("OTEL_EXPORTER_OTLP_PROTOCOL");

        if (!decimal.TryParse(limitAmountTransferVariable, out var limitAmountTransfer))
            throw new ArgumentException(limitAmountTransferVariable);   

        if (string.IsNullOrEmpty(bankAccountBaseAddress))
            throw new ArgumentNullException(nameof(bankAccountBaseAddress));    
        
        if (string.IsNullOrEmpty(openTelemetryEndpoint))
            throw new ArgumentNullException(nameof(openTelemetryEndpoint));

        if (string.IsNullOrEmpty(openTelemetryProtocol))
            throw new ArgumentNullException(nameof(openTelemetryProtocol));
        
        var apiConfiguration = new ApiConfiguration()
        {
            Title = "Bank Transactions API",
            Description = "Bank Transactions API provides a methods to handle transactions",
        };

        builder.AddCommonsOpenTelemetry(
            "transaction-api",
            openTelemetryEndpoint, 
            openTelemetryProtocol);

        builder.Services.AddControllers();
        builder.Services.AddCommonsApi(apiConfiguration);
        builder.Services.AddBankTransactions(
            bankAccountBaseAddress,
            limitAmountTransfer);

        var app = builder.Build();

        app.UseCommonsApi();

        app.UseHttpsRedirection();

        app.UseAuthorization();

        app.MapControllers();

        await app.RunAsync();
    }
}