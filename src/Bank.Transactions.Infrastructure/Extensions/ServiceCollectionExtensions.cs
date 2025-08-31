using System.Collections.Concurrent;
using Bank.Commons.Applications.Factories.Results;
using Bank.Commons.Applications.Serializers;
using Bank.Transactions.Application.Gateways;
using Bank.Transactions.Application.Repositories;
using Bank.Transactions.Application.Services;
using Bank.Transactions.Application.UseCases.CreateTransaction;
using Bank.Transactions.Application.UseCases.ExecuteTransaction;
using Bank.Transactions.Application.UseCases.GetTransactionsBalance;
using Bank.Transactions.Application.UseCases.GetTransactionsHistory;
using Bank.Transactions.Domain.Entities;
using Bank.Transactions.Infrastructure.Gateways;
using Bank.Transactions.Infrastructure.Repositories;
using Confluent.Kafka;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;
using MongoDB.Driver.Core.Extensions.DiagnosticSources;

namespace Bank.Transactions.Infrastructure.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddBankTransactions(
        this IServiceCollection services,
        BankTransactionConfigure configuration)
        => services
            .AddBankApplication(configuration)
            .AddBankInfrastructure(configuration)
            .AddCommons();

    private static IServiceCollection AddBankApplication(
        this IServiceCollection services, BankTransactionConfigure configuration)
        => services
            .AddSingleton(new TransferParameters()
                { LimitAmountTransfer = configuration.LimitAmountTransfer })
            .AddSingleton(new ConcurrentDictionary<Guid, SemaphoreSlim>())
            .AddBankApplicationUseCases()
            .AddBankApplicationServices()
            .AddBankApplicationValidator();

    private static IServiceCollection AddBankInfrastructure(
        this IServiceCollection services,
        BankTransactionConfigure configuration)
        => services
            .AddBankInfrastructureGateways(configuration.BankAccountBaseAddress)
            .AddBankInfrastructureKafka(configuration.MessageQueueHost, configuration.MessageGroupId)
            .AddBankInfrastructureRepositories(
                configuration.TransactionConnectionString,
                configuration.TransactionDatabaseName);

    private static IServiceCollection AddCommons(
        this IServiceCollection services)
        => services
            .AddSingleton<IResultFactory, ResultFactory>()
            .AddSingleton<IJsonSerializer, JsonSerializerDefault>();

    private static IServiceCollection AddBankApplicationUseCases(this IServiceCollection services)
        => services
            .AddScoped<IExecuteTransactionUseCase, ExecuteTransactionUseCase>()
            .AddScoped<IGetTransactionsBalanceUseCase, GetTransactionsBalanceUseCase>()
            .AddScoped<IGetTransactionsHistoryUseCase, GetTransactionsHistoryUseCase>()
            .AddScoped<ICreateTransactionUseCase, CreateTransactionUseCase>()
            .AddSingleton<ICreateTransactionInputMapper, CreateTransactionInputMapper>();

    private static IServiceCollection AddBankApplicationServices(this IServiceCollection services)
        => services
            .AddScoped<IAmountService, AmountService>()
            .AddScoped<ITransactionService, TransactionService>();

    private static IServiceCollection AddBankApplicationValidator(this IServiceCollection services)
        => services
            .AddValidatorsFromAssemblyContaining<IAmountService>();

    private static IServiceCollection AddBankInfrastructureGateways(
        this IServiceCollection services,
        string bankAccountBaseAddress)
    {
        services
            .AddHttpClient<IBankAccountClient, BankAccountClient>(httpClient =>
                httpClient.BaseAddress = new Uri(bankAccountBaseAddress))
            .AddStandardResilienceHandler();

        return services;
    }

    private static IServiceCollection AddBankInfrastructureKafka(
        this IServiceCollection services, string messageQueueHost, string messageGroupId)
        => services
            .AddSingleton(
                new ProducerConfig { BootstrapServers = messageQueueHost })
            .AddSingleton(
                new ConsumerConfig
                {
                    BootstrapServers = messageQueueHost,
                    GroupId = messageGroupId
                })
            .AddSingleton(sp => new ProducerBuilder<string, string>(
                sp.GetRequiredService<ProducerConfig>())
                .Build())
            .AddSingleton(sp => new ConsumerBuilder<string, string>(
                sp.GetRequiredService<ConsumerConfig>())
                .Build())
            .AddSingleton<ITransactionProducer, TransactionProducer>()
            .AddSingleton<ITransactionConsumer, TransactionConsumer>();
        


    private static IServiceCollection AddBankInfrastructureRepositories(
        this IServiceCollection services,
        string connectionString,
        string databaseName)
        => services
            .AddSingleton<IMongoClient>(
                CreateMongoClient(connectionString))
            .AddSingleton<IMongoDatabase>(sp => sp.GetRequiredService<IMongoClient>()
                .MappingEntities()
                .GetDatabase(databaseName))
            .AddSingleton<ITransactionRepository, TransactionRepository>();

    private static MongoClient CreateMongoClient(string connectionString)
    {
        var clientSettings = MongoClientSettings.FromConnectionString(connectionString);
        var options = new InstrumentationOptions { CaptureCommandText = true };
        clientSettings.ClusterConfigurator = cb =>
            cb.Subscribe(new DiagnosticsActivityEventSubscriber(options));
        return new MongoClient(clientSettings);
    }
}