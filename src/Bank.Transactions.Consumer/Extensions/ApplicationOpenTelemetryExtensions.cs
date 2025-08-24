using Bank.Transactions.Consumer.OpenTelemetry;
using Microsoft.Extensions.DependencyInjection;
using OpenTelemetry;
using OpenTelemetry.Exporter;
using OpenTelemetry.Logs;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

namespace Bank.Transactions.Consumer.Extensions;

public static class ApplicationOpenTelemetryExtensions
{
    public static IServiceCollection AddBankTransactionsOpenTelemetry(
        this IServiceCollection services,
        string serviceName,
        string endpoint, 
        string protocol)
    {
        Action<OtlpExporterOptions> configureExporter = (options) =>
        {
            options.Protocol = GetOpenTelemetryProtocol(protocol);
            options.Endpoint = new Uri(endpoint);
        };
        
        services.AddOpenTelemetry()

            .ConfigureResource(builder
                => builder.AddService(serviceName: serviceName))

            .WithTracing(builder
                => builder
                    .AddSource(serviceName)
                    .AddSource("MongoDB.Driver.Core.Extensions.DiagnosticSources")
                    .AddHttpClientInstrumentation()
                    .AddOtlpExporter(configureExporter))

            .WithMetrics(builder
                => builder
                    .AddMeter(serviceName)
                    .AddHttpClientInstrumentation()
                    .AddOtlpExporter(configureExporter))

            .WithLogging(builder
                => builder
                    .AddApplicationExporter()
                    .AddOtlpExporter(configureExporter));
        
        return services;
    }
    
    private static OtlpExportProtocol GetOpenTelemetryProtocol(string protocol) =>
        protocol.ToLower() switch
        {
            "http"  => OtlpExportProtocol.HttpProtobuf,
            "http/protobuf" => OtlpExportProtocol.HttpProtobuf,
            _ => OtlpExportProtocol.Grpc
        };

    private static LoggerProviderBuilder AddApplicationExporter(this LoggerProviderBuilder builder)
        => builder
            .AddProcessor(
                new SimpleLogRecordExportProcessor(
                    new AplicationExporter()));
}