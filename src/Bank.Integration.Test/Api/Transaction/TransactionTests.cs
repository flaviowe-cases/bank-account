using System.Net.Http.Json;
using Bank.Accounts.Api;
using Bank.Integration.Test.Factories;
using Bank.Transactions.Api.Controllers.ExecuteTransaction;
using FluentAssertions;

namespace Bank.Integration.Test.Api.Transaction;

public class TransactionTests : 
    IClassFixture<ApiFactory<Program>>,
    IClassFixture<ApiFactory<Bank.Transactions.Api.Program>>
{
    private readonly ApiFactory<Transactions.Api.Program> _transactionApiFactory;

    public TransactionTests(ApiFactory<Program> accountApiFactory,
        ApiFactory<Bank.Transactions.Api.Program> transactionApiFactory)
    {
        _transactionApiFactory = transactionApiFactory;

        _transactionApiFactory.BankAccountClientMessageHandler = () 
            => accountApiFactory.Server.CreateHandler();
        
        accountApiFactory.BankTransactionClientMessageHandler = () 
            => _transactionApiFactory.Server.CreateHandler();
    }

    [Fact]
    public async Task AddTransaction_Should_Create_Transaction()
    {
        var httpClient = _transactionApiFactory.CreateClient();

        var request = new ExecuteTransactionRequest()
        {
            DestinationAccountNumber = 10002,
            Amount = 15000,
        };
        
        var response = await httpClient.PostAsJsonAsync($"api/v1/Transaction", request);
        response.IsSuccessStatusCode.Should().BeTrue(); 
    }
}