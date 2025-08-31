using Bank.Commons.Api.OpenTelemetry;
using Confluent.Kafka;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Npgsql;
using OpenTelemetry.Exporter;
using OpenTelemetry.Logs;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using SimpleLogRecordExportProcessor = OpenTelemetry.SimpleLogRecordExportProcessor;

namespace Bank.Commons.Api.Extensions;

public static class OpenTelemetryExtensions
{
    public static WebApplicationBuilder AddCommonsOpenTelemetry(
        this WebApplicationBuilder appBuilder,
        string serviceName,
        string endpoint,
        string protocol,
        string? messageQueueHost = null)
    {
        Action<OtlpExporterOptions> configureExporter = (options) =>
        {
            options.Protocol = GetOpenTelemetryProtocol(protocol);
            options.Endpoint = new Uri(endpoint);
        };

        appBuilder.Logging.ClearProviders();

        if (!string.IsNullOrWhiteSpace(messageQueueHost))
            appBuilder.Services.AddKafkaBuilders(serviceName, messageQueueHost);

        appBuilder.Services
            .AddOpenTelemetry()
            .ConfigureResource(builder
                => builder.AddService(serviceName: serviceName))
            .WithTracing(builder =>
            {
                builder
                    .AddSource(serviceName)
                    .AddSource("MongoDB.Driver.Core.Extensions.DiagnosticSources")
                    .AddNpgsql()
                    .AddHttpClientInstrumentation()
                    .AddAspNetCoreInstrumentation()
                    .AddOtlpExporter(configureExporter);

                if (!string.IsNullOrWhiteSpace(messageQueueHost))
                    builder
                        .AddKafkaProducerInstrumentation<string, string>()
                        .AddKafkaConsumerInstrumentation<string, string>();
            })
            .WithMetrics(builder =>
            {
                builder
                    .AddMeter(serviceName)
                    .AddHttpClientInstrumentation()
                    .AddAspNetCoreInstrumentation()
                    .AddOtlpExporter(configureExporter);
                
                if (!string.IsNullOrWhiteSpace(messageQueueHost))
                    builder
                        .AddKafkaProducerInstrumentation<string, string>()
                        .AddKafkaConsumerInstrumentation<string, string>();
            })
            .WithLogging(builder
                => builder
                    .AddApiExporter()
                    .AddOtlpExporter(configureExporter));

        return appBuilder;
    }

    private static OtlpExportProtocol GetOpenTelemetryProtocol(string protocol) =>
        protocol.ToLower() switch
        {
            "http" => OtlpExportProtocol.HttpProtobuf,
            "http/protobuf" => OtlpExportProtocol.HttpProtobuf,
            _ => OtlpExportProtocol.Grpc
        };

    private static void AddKafkaBuilders(
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
    }

    private static LoggerProviderBuilder AddApiExporter(this LoggerProviderBuilder builder)
        => builder
            .AddProcessor(
                new SimpleLogRecordExportProcessor(
                    new ApiExporter()));
}