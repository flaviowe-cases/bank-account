using Bank.Accounts.Application.Gateways;
using Bank.Accounts.Application.Mappers;
using Bank.Accounts.Application.Repositories;
using Bank.Accounts.Application.Services.Amounts;
using Bank.Accounts.Application.UseCases.AddAccount;
using Bank.Accounts.Application.UseCases.GetAccount;
using Bank.Accounts.Application.UseCases.ListAccounts;
using Bank.Accounts.Infrastructure.Gateways;
using Bank.Accounts.Infrastructure.Repositories;
using Bank.Commons.Applications.Factories.Results;
using Bank.Commons.Applications.Serializers;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Bank.Accounts.Infrastructure.Extensions;

public static class ServiceCollectionsExtensions
{
    public static IServiceCollection AddBankAccounts(
        this IServiceCollection services,
        string bankTransactionBaseAddress,
        string accountConnectionString)
        => services
            .AddApplication()
            .AddInfrastructure(
                bankTransactionBaseAddress,
                accountConnectionString)
            .AddCommons();

    private static IServiceCollection AddApplication(this IServiceCollection services)
        => services
            .AddApplicationMappers()
            .AddApplicationServices()
            .AddUseCases()
            .AddApplicationValidator();

    private static IServiceCollection AddInfrastructure(
        this IServiceCollection services, 
        string bankTransactionBaseAddress,
        string accountConnectionString)
        => services
            .AddInfrastructureGateways(bankTransactionBaseAddress)   
            .AddInfrastructureRepositoriesPostgres(accountConnectionString);

    private static IServiceCollection AddCommons(
        this IServiceCollection services)
        => services
            .AddSingleton<IResultFactory, ResultFactory>()
            .AddSingleton<IJsonSerializer, JsonSerializerDefault>();
    
    private static IServiceCollection AddApplicationMappers(this IServiceCollection services)
        => services
            .AddScoped<IAccountApplicationMapper, AccountApplicationMapper>();
    
    private static IServiceCollection AddApplicationServices(this IServiceCollection services)
        => services
            .AddScoped<IAmountService, AmountService>();    
    
    private static IServiceCollection AddUseCases(this IServiceCollection services)
        => services
            .AddScoped<IAddAccountUseCase, AddAccountUseCase>()
            .AddSingleton<IAddAccountInputMapper, AddAccountInputMapper>()  
            .AddScoped<IGetAccountUseCase, GetAccountUseCase>()
            .AddScoped<IGetAccountOutputMapper, GetAccountOutputMapper>()
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

    private static IServiceCollection AddInfrastructureRepositoriesPostgres(
        this IServiceCollection services,
        string connectionString)
        => services
            .AddEntityFrameworkNpgsql()
            .AddDbContext<AccountContext>((sp, options) 
                => options
                    .UseNpgsql(connectionString)
                    .UseInternalServiceProvider(sp))
            .AddScoped<IAccountRepository, AccountRepository>();
}