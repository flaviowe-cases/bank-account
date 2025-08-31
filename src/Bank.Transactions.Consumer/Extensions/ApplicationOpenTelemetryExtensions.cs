using Bank.Transactions.Consumer.OpenTelemetry;
using Confluent.Kafka;
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
        string protocol,
        string messageQueueHost)
    {
        Action<OtlpExporterOptions> configureExporter = (options) =>
        {
            options.Protocol = GetOpenTelemetryProtocol(protocol);
            options.Endpoint = new Uri(endpoint);
        };

        services
            .AddLogging()
            .AddKafkaBuilders(serviceName, messageQueueHost)
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
        this IServiceCollection services,
        string serviceName,
        string messageQueueHost)
    {
        var producerConfig = new ProducerConfig()
        {
            BootstrapServers = messageQueueHost,
        };

        var consumerConfig = new ConsumerConfig()
        {
            BootstrapServers = messageQueueHost,
            GroupId = serviceName,
            AutoOffsetReset = AutoOffsetReset.Earliest,
            EnablePartitionEof = true,
        };

        services
            .AddSingleton(new InstrumentedProducerBuilder<string, string>(producerConfig))
            .AddSingleton(new InstrumentedConsumerBuilder<string, string>(consumerConfig));

        return services;
    }

    private static LoggerProviderBuilder AddApplicationExporter(this LoggerProviderBuilder builder)
        => builder
            .AddProcessor(
                new SimpleLogRecordExportProcessor(
                    new AplicationExporter()));
}