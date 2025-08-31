using Confluent.Kafka;
using Microsoft.Extensions.DependencyInjection;
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

        services
            .AddLogging()
            .AddKafkaBuilders()
            .AddOpenTelemetry()
            .ConfigureResource(builder
                => builder.AddService(serviceName: serviceName))
            .WithTracing(builder
                => builder
                    .AddSource(serviceName)
                    .AddSource("MongoDB.Driver.Core.Extensions.DiagnosticSources")
                    .AddHttpClientInstrumentation()
                    .AddKafkaProducerInstrumentation<string, string>()
                    .AddKafkaConsumerInstrumentation<string, string>()
                    .AddOtlpExporter(configureExporter))
            .WithMetrics(builder
                => builder
                    .AddMeter(serviceName)
                    .AddHttpClientInstrumentation()
                    .AddKafkaProducerInstrumentation<string, string>()
                    .AddKafkaConsumerInstrumentation<string, string>()
                    .AddOtlpExporter(configureExporter))
            .WithLogging(builder
                => builder
                    .AddOtlpExporter(configureExporter));

        return services;
    }

    private static OtlpExportProtocol GetOpenTelemetryProtocol(string protocol) =>
        protocol.ToLower() switch
        {
            "http" or "http/protobuf" => OtlpExportProtocol.HttpProtobuf,
            _ => OtlpExportProtocol.Grpc
        };

    private static IServiceCollection AddKafkaBuilders(
        this IServiceCollection services)
    {
        services
            .AddSingleton(sp =>
                new InstrumentedProducerBuilder<string, string>(
                    sp.GetRequiredService<ProducerConfig>()))
            .AddSingleton(sp => 
                new InstrumentedConsumerBuilder<string, string>(
                    sp.GetRequiredService<ConsumerConfig>()));

        return services;
    }
}