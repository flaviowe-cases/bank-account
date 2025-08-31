using Bank.Transactions.Infrastructure.Gateways.KafkaBroker;
using Microsoft.Extensions.DependencyInjection;

namespace Bank.Transactions.Infrastructure.Extensions;

public static class ServiceProviderExtensions
{
    public static Task ConfigureBankTransactionsAsync(this IServiceProvider serviceProvider)
        => serviceProvider
            .GetRequiredService<ITopicCreators>()
            .CreateTopicsAsync();
}