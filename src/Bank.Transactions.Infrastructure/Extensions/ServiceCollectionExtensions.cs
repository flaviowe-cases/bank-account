using System.Collections.Concurrent;
using Bank.Transactions.Application.Factories.Results;
using Bank.Transactions.Application.Gateways;
using Bank.Transactions.Application.Repositories;
using Bank.Transactions.Application.Serializers;
using Bank.Transactions.Application.Services;
using Bank.Transactions.Application.UseCases.ExecuteTransaction;
using Bank.Transactions.Domain.Entities;
using Bank.Transactions.Infrastructure.Gateways;
using Bank.Transactions.Infrastructure.Repositories;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Bank.Transactions.Infrastructure.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddBankTransactions(
        this IServiceCollection services, string bankAccountBaseAddress, decimal limitAmountTransfer)
        => services
            .AddBankApplication(limitAmountTransfer)
            .AddBankInfrastructure(bankAccountBaseAddress);

    private static IServiceCollection AddBankApplication(
        this IServiceCollection services, decimal limitAmountTransfer)
        => services
            .AddSingleton(new TransferParameters() { LimitAmountTransfer = limitAmountTransfer })
            .AddSingleton(new ConcurrentDictionary<Guid, SemaphoreSlim>())
            .AddBankApplicationUseCases()
            .AddBankApplicationFactories()
            .AddBankApplicationServices()
            .AddBankApplicationSerializers()
            .AddBankApplicationValidator();

    private static IServiceCollection AddBankInfrastructure(
        this IServiceCollection services,
        string bankAccountBaseAddress)
        => services
            .AddBankInfrastructureGateways(bankAccountBaseAddress)
            .AddBankInfrastructureRepositories();

    private static IServiceCollection AddBankApplicationUseCases(this IServiceCollection services)
        => services
            .AddScoped<IExecuteTransactionUseCase, ExecuteTransactionUseCase>();

    private static IServiceCollection AddBankApplicationFactories(this IServiceCollection services)
        => services
            .AddSingleton<IResultFactory, ResultFactory>();

    private static IServiceCollection AddBankApplicationSerializers(this IServiceCollection services)
        => services
            .AddSingleton<IJsonSerializer, JsonSerializerDefault>();
    
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

    private static IServiceCollection AddBankInfrastructureRepositories(this IServiceCollection services)
        => services
            .AddEntityFrameworkInMemoryDatabase()
            .AddDbContext<TransactionContext>((sp, options) => options
                .UseInMemoryDatabase("transaction")
                .UseApplicationServiceProvider(sp))
            .AddScoped<ITransactionRepository, TransactionRepository>();
}