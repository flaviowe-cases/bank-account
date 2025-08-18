using System.Net.Http.Json;
using Bank.Accounts.Api;
using Bank.Accounts.Api.Controllers.AddAccount;
using Bank.Accounts.Application.UseCases.GetAccount;
using Bank.Integration.Test.Factories;
using FluentAssertions;

namespace Bank.Integration.Test.Api.Accounts;

public class AccountTests : IClassFixture<ApiFactory<Program>>, IClassFixture<ApiFactory<Bank.Transactions.Api.Program>>
{
    private readonly ApiFactory<Program> _accountApiFactory;

    public AccountTests(ApiFactory<Program> accountApiFactory,
        ApiFactory<Bank.Transactions.Api.Program> transactionApiFactory)
    {
        _accountApiFactory = accountApiFactory;

        _accountApiFactory.BankTransactionClientMessageHandler = () 
            => transactionApiFactory.Server.CreateHandler();

        transactionApiFactory.BankAccountClientMessageHandler = () 
            => _accountApiFactory.Server.CreateHandler();
    }

    [Fact]
    public async Task GetAccount_Should_Returns_Account()
    {
        using var httpClient = _accountApiFactory.CreateClient();
        const int accountNumber = 10001;
        var url = $"api/v1/Account/{accountNumber}";

        var accountResponse = await httpClient.GetFromJsonAsync<GetAccountOutput>(url);

        accountResponse.Should().NotBeNull();
        accountResponse.Account.Should().NotBeNull();
        accountResponse.Account.Number.Should().Be(accountNumber);
    }
    
    [Fact]
    public async Task AddAccount_Should_Returns_Created()
    {
        using var httpClient = _accountApiFactory.CreateClient();
        const string url = $"api/v1/Account";
        
        var request = new AddAccountRequest()
        {
            AccountId = Guid.NewGuid(),
            Name = "Test Account",
            AccountNumber = 123456,
            InitialAmount = 15000
        };
        
        var response = await httpClient.PostAsJsonAsync(url, request);
        response.IsSuccessStatusCode.Should().BeTrue(); 
    }
}