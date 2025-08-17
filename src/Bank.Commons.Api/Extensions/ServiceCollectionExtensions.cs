using Microsoft.Extensions.DependencyInjection;

namespace Bank.Commons.Api.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddCommonsApi(this IServiceCollection services, ApiConfiguration apiConfiguration)
        => services
            .AddSingleton(apiConfiguration);

}