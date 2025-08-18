using Bank.Commons.Api.Swagger;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Bank.Commons.Api.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddCommonsApi(this IServiceCollection services, ApiConfiguration apiConfiguration)
        => services
            .AddSingleton(apiConfiguration)
            .AddSwaggerGen()
            .AddTransient<IConfigureOptions<SwaggerGenOptions>, ConfigureSwaggerOptions>()
            .AddCommonsApiVersioning();

    private static IServiceCollection AddCommonsApiVersioning(this IServiceCollection services)
    {
        services
            .AddApiVersioning(options => { options.ReportApiVersions = true; })
            .AddApiExplorer(options =>
            {
                options.GroupNameFormat = "'v'VVV";
                options.SubstituteApiVersionInUrl = true;
            });

        return services;
    }
}