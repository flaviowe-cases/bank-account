using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using OpenTelemetry.Exporter;
using OpenTelemetry.Logs;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

namespace Bank.Commons.Api.Extensions;

public static class OpenTelemetryExtensions
{
    private static readonly Action<OtlpExporterOptions> ConfigureExporter = (options) =>
    {
        options.Protocol = OtlpExportProtocol.Grpc;
        options.Endpoint = new Uri("http://localhost:4317");
    };
    
    public static WebApplicationBuilder AddCommonsOpenTelemetry(this WebApplicationBuilder appBuilder,
        string serviceName)
    {
        appBuilder.Logging.ClearProviders();

        appBuilder.Services.AddOpenTelemetry()

            .ConfigureResource(builder
                => builder.AddService(serviceName: serviceName))

            .WithTracing(builder
                => builder
                    .AddHttpClientInstrumentation()
                    .AddAspNetCoreInstrumentation()
                    .AddOtlpExporter(ConfigureExporter))

            .WithMetrics(builder
                => builder
                    .AddHttpClientInstrumentation()
                    .AddAspNetCoreInstrumentation()
                    .AddOtlpExporter(ConfigureExporter))

            .WithLogging(builder
                => builder
                    .AddOtlpExporter(ConfigureExporter));
            
        return appBuilder;
    }
        

}