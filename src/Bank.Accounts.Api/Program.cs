using Bank.Accounts.Infrastructure.Extensions;
using Bank.Accounts.Infrastructure.Stubs;
using Bank.Commons.Api;
using Bank.Commons.Api.Extensions;

namespace Bank.Accounts.Api;

public class Program
{
    public static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        
        const string serviceName = "account-api";

        var bankTransactionBaseAddress = Environment.GetEnvironmentVariable("BANK_TRANSACTION_BASE_ADDRESS");
        var openTelemetryEndpoint = Environment.GetEnvironmentVariable("OTEL_EXPORTER_OTLP_ENDPOINT");
        var openTelemetryProtocol = Environment.GetEnvironmentVariable("OTEL_EXPORTER_OTLP_PROTOCOL");
        var accountConnectionString = Environment.GetEnvironmentVariable("ACCOUNT_DB_CONNECTION_STRING");

        if (string.IsNullOrEmpty(bankTransactionBaseAddress))
            throw new ArgumentNullException(nameof(bankTransactionBaseAddress));

        if (string.IsNullOrEmpty(openTelemetryEndpoint))
            throw new ArgumentNullException(nameof(openTelemetryEndpoint));

        if (string.IsNullOrEmpty(openTelemetryProtocol))
            throw new ArgumentNullException(nameof(openTelemetryProtocol));

        if (string.IsNullOrEmpty(accountConnectionString))
            throw new ArgumentNullException(nameof(accountConnectionString));

        var apiConfiguration = new ApiConfiguration()
        {
            Title = "Bank Accounts API",
            Description = "Bank Accounts API provides a methods to handle accounts",
        };

        builder.AddCommonsOpenTelemetry(
            serviceName,
            openTelemetryEndpoint,
            openTelemetryProtocol);

        builder.Services.AddControllers();
        builder.Services.AddCommonsApi(apiConfiguration);
        builder.Services.AddBankAccounts(
            bankTransactionBaseAddress,
            accountConnectionString);

        builder.Services.AddScoped<IAccountStubs, AccountStubs>();

        var app = builder.Build();
        await app.Services.ConfigureBankAccountAsync();
        
        app.UseCommonsApi();
        app.UseHttpsRedirection();
        app.MapControllers();
        
        await app.RunAsync();
    }
}