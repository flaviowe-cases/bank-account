using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Npgsql;
using OpenTelemetry.Exporter;
using OpenTelemetry.Logs;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

namespace Bank.Commons.Api.Extensions;

public static class OpenTelemetryExtensions
{
    public static WebApplicationBuilder AddCommonsOpenTelemetry(this WebApplicationBuilder appBuilder,
        string serviceName, string endpoint, string protocol)
    {
        Action<OtlpExporterOptions> configureExporter = (options) =>
        {
            options.Protocol = GetOpenTelemetryProtocol(protocol);
            options.Endpoint = new Uri(endpoint);
        };
        
        appBuilder.Logging.ClearProviders();

        appBuilder.Services.AddOpenTelemetry()

            .ConfigureResource(builder
                => builder.AddService(serviceName: serviceName))

            .WithTracing(builder
                => builder
                    .AddSource(serviceName)
                    .AddSource("MongoDB.Driver.Core.Extensions.DiagnosticSources")
                    .AddNpgsql()
                    .AddHttpClientInstrumentation()
                    .AddAspNetCoreInstrumentation()
                    .AddOtlpExporter(configureExporter))

            .WithMetrics(builder
                => builder
                    .AddMeter(serviceName)
                    .AddHttpClientInstrumentation()
                    .AddAspNetCoreInstrumentation()
                    .AddOtlpExporter(configureExporter))

            .WithLogging(builder
                => builder
                    .AddOtlpExporter(configureExporter)
                    .AddConsoleExporter());
            
        return appBuilder;
    }

    private static OtlpExportProtocol GetOpenTelemetryProtocol(string protocol) =>
        protocol.ToLower() switch
        {
            "http"  => OtlpExportProtocol.HttpProtobuf,
            "http/protobuf" => OtlpExportProtocol.HttpProtobuf,
            _ => OtlpExportProtocol.Grpc
        };
}