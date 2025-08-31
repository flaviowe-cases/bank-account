using Microsoft.Extensions.DependencyInjection;

namespace Bank.Commons.Api.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddCommonsApi(this IServiceCollection services, ApiConfiguration apiConfiguration)
        => services
            .AddSingleton(apiConfiguration)
            .AddOpenApi(options =>
            {
                options.AddDocumentTransformer<OpenApiDocumentTransformer>();
            })
            .AddCommonsApiVersioning();

    private static IServiceCollection AddCommonsApiVersioning(this IServiceCollection services)
    {
        services
            .AddApiVersioning(options =>
            {
                options.ReportApiVersions = true;
            })
            .AddApiExplorer(options =>
            {
                options.GroupNameFormat = "'v'VVV";
                options.SubstituteApiVersionInUrl = true;
            });

        return services;
    }
}