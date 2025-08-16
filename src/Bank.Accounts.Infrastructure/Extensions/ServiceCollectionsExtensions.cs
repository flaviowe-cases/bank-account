using Bank.Accounts.Application.Factories.Results;
using Bank.Accounts.Application.Gateways;
using Bank.Accounts.Application.Mappers;
using Bank.Accounts.Application.Repositories;
using Bank.Accounts.Application.Serializers;
using Bank.Accounts.Application.Services.Amounts;
using Bank.Accounts.Application.UseCases.AddAccount;
using Bank.Accounts.Application.UseCases.GetAccount;
using Bank.Accounts.Application.UseCases.ListAccounts;
using Bank.Accounts.Infrastructure.Gateways;
using Bank.Accounts.Infrastructure.Repositories;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Bank.Accounts.Infrastructure.Extensions;

public static class ServiceCollectionsExtensions
{
    public static IServiceCollection AddBankAccounts(
        this IServiceCollection services,
        string bankTransactionBaseAddress)
        => services
            .AddApplication()
            .AddInfrastructure(bankTransactionBaseAddress);

    private static IServiceCollection AddApplication(this IServiceCollection services)
        => services
            .AddApplicationFactories()
            .AddApplicationMappers()
            .AddApplicationSerializers()
            .AddApplicationServices()
            .AddUseCases()
            .AddApplicationValidator();

    private static IServiceCollection AddInfrastructure(
        this IServiceCollection services, string bankTransactionBaseAddress)
        => services
            .AddInfrastructureGateways(bankTransactionBaseAddress)   
            .AddInfrastructureRepositories();

    private static IServiceCollection AddApplicationFactories(this IServiceCollection services)
        => services
            .AddSingleton<IResultFactory, ResultFactory>();
    
    private static IServiceCollection AddApplicationMappers(this IServiceCollection services)
        => services
            .AddScoped<IAccountApplicationMapper, AccountApplicationMapper>();
    
    private static IServiceCollection AddApplicationSerializers(this IServiceCollection services)
        => services
            .AddSingleton<IJsonSerializer, JsonSerializerDefault>();
    
    private static IServiceCollection AddApplicationServices(this IServiceCollection services)
        => services
            .AddScoped<IAmountService, AmountService>();    
    
    private static IServiceCollection AddUseCases(this IServiceCollection services)
        => services
            .AddScoped<IAddAccountUseCase, AddAccountUseCase>()
            .AddSingleton<IAddAccountInputMapper, AddAccountInputMapper>()  
            .AddScoped<IGetAccountUseCase, GetAccountUseCase>()
            .AddScoped<IListAccountsUseCase, ListAccountsUseCase>();
    
    private static IServiceCollection AddApplicationValidator(this IServiceCollection services)
        => services
            .AddValidatorsFromAssemblyContaining<IAddAccountInputMapper>();

    private static IServiceCollection AddInfrastructureGateways(
        this IServiceCollection services, string bankTransactionBaseAddress)
    {
        services
            .AddHttpClient<IBankTransactionsClient, BankTransactionClient>(client =>
                client.BaseAddress = new Uri(bankTransactionBaseAddress))
            .AddStandardResilienceHandler();
        
        return services;
    }

    private static IServiceCollection AddInfrastructureRepositories(this IServiceCollection services)
        => services
            .AddEntityFrameworkInMemoryDatabase()
            .AddDbContext<AccountContext>((sp, options) => options
                .UseInMemoryDatabase("account")
                .UseInternalServiceProvider(sp))
            .AddScoped<IAccountRepository, AccountRepository>();
}