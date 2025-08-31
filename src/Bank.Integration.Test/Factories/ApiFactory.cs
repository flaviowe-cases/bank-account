using Bank.Accounts.Application.Gateways;
using Bank.Accounts.Infrastructure.Gateways;
using Bank.Transactions.Application.Gateways;
using Bank.Transactions.Infrastructure.Gateways;
using Bank.Transactions.Infrastructure.Gateways.AccountApi;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Bank.Integration.Test.Factories;

public class ApiFactory<TEntryPoint> : WebApplicationFactory<TEntryPoint> where TEntryPoint : class
{
    public Func<HttpMessageHandler>? BankTransactionClientMessageHandler;
    public Func<HttpMessageHandler>? BankAccountClientMessageHandler;

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Test");

        SetEnvironmentVariables();

        builder.ConfigureTestServices(services =>
        {
            if (BankTransactionClientMessageHandler != null)
                services
                    .RemoveAll<IBankTransactionsClient>()
                    .AddHttpClient<IBankTransactionsClient, BankTransactionClient>()
                    .ConfigurePrimaryHttpMessageHandler(BankTransactionClientMessageHandler);

            if (BankAccountClientMessageHandler != null)
                services
                    .RemoveAll<IBankAccountClient>()
                    .AddHttpClient<IBankAccountClient, BankAccountClient>()
                    .ConfigurePrimaryHttpMessageHandler(BankAccountClientMessageHandler);
        });

        base.ConfigureWebHost(builder);
    }

    private static void SetEnvironmentVariables()
    {
        const string baseAddress = "http://localhost";
        const string limitAmountTransfer = "10000";

        Environment.SetEnvironmentVariable("BANK_ACCOUNT_BASE_ADDRESS", baseAddress);
        Environment.SetEnvironmentVariable("BANK_TRANSACTION_BASE_ADDRESS", baseAddress);
        Environment.SetEnvironmentVariable("LIMIT_AMOUNT_TRANSFER", limitAmountTransfer);
    }
}