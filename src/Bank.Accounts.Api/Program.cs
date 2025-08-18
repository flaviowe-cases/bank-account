using Bank.Accounts.Infrastructure.Extensions;
using Bank.Commons.Api;
using Bank.Commons.Api.Extensions;

namespace Bank.Accounts.Api;

public class Program
{
    public static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        var bankTransactionBaseAddress = Environment.GetEnvironmentVariable("BANK_TRANSACTION_BASE_ADDRESS");

        if (string.IsNullOrEmpty(bankTransactionBaseAddress))
            throw new ArgumentNullException(nameof(bankTransactionBaseAddress));

        var apiConfiguration = new ApiConfiguration()
        {
            Title = "Bank Accounts API",
            Description = "Bank Accounts API provides a methods to handle accounts",
        };

        builder.AddCommonsOpenTelemetry("account-api");

        builder
            .Services
            .AddCommonsApi(apiConfiguration)
            .AddBankAccounts(bankTransactionBaseAddress)
            .AddControllers();

        builder.Services.AddScoped<IAccountStubs, AccountStubs>();

        var app = builder.Build();

        app.UseCommonsApi();

        app.UseHttpsRedirection();

        app.MapControllers();

        using (var scope = app.Services.CreateScope())
            await scope.ServiceProvider.GetRequiredService<IAccountStubs>()
                .AddAccountsAsync();

        await app.RunAsync();
    }
}