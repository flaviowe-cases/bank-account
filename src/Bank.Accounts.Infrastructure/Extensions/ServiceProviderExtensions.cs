using Bank.Accounts.Infrastructure.Stubs;
using Microsoft.Extensions.DependencyInjection;

namespace Bank.Accounts.Infrastructure.Extensions;

public static class ServiceProviderExtensions
{
    public static Task ConfigureBankAccountAsync(this IServiceProvider serviceProvider)
        => serviceProvider
            .CreateScope().ServiceProvider
            .GetRequiredService<IAccountStubs>()
            .AddAccountsAsync();
}