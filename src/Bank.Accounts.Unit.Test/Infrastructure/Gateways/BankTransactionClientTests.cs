using System.Net;
using AutoFixture;
using Bank.Accounts.Application.Models;
using Bank.Accounts.Infrastructure.Gateways;
using Bank.Accounts.Unit.Test.Stubs;
using Bank.Commons.Applications.Factories.Results;
using Bank.Commons.Applications.Serializers;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using NSubstitute;

namespace Bank.Accounts.Unit.Test.Infrastructure.Gateways;

public class BankTransactionClientTests
{
    [Fact]
    public async Task GetAmountsAsync_Should_Return_AmountApplicationList()
    {
        //Arrange
        var fixture = new Fixture();

        var responseBody = fixture.Create<string>();
        var httpClientHandler = new HttpClientHandlerStub(HttpStatusCode.OK, responseBody);

        var logger = Substitute.For<ILogger<BankTransactionClient>>();
        
        var httpClient = new HttpClient(httpClientHandler) 
            {BaseAddress = new Uri("https://localhost")};
        
        var jsonSerializer = Substitute.For<IJsonSerializer>();
        var resultFactory = Substitute.For<IResultFactory>();

        var bankTransactionClient = new BankTransactionClient(
            logger, httpClient, jsonSerializer, resultFactory);

        var accountsBalance = fixture
            .CreateMany<AccountBalanceApplication>()
            .ToList();

        var getAccountBalanceResponse = fixture.Build<GetAccountBalanceResponse>()
            .With(response => response.AccountBalance, accountsBalance)
            .Create();

        var accountBalancesResult = new Result<List<AccountBalanceApplication>>(accountsBalance)
            { Success = true };

        var accountsId = fixture.CreateMany<Guid>()
            .ToList();
        
        jsonSerializer
            .Deserialize<GetAccountBalanceResponse>(responseBody)
            .Returns(getAccountBalanceResponse);

        resultFactory
            .CreateSuccess(accountsBalance)
            .Returns(accountBalancesResult);

        //Act
        var result = await bankTransactionClient.GetAccountsBalanceAsync(accountsId);

        //Assert
        result.Should().NotBeNull();
        result.Success.Should().BeTrue();
        result.GetContent().Should().BeEquivalentTo(accountsBalance);
    }
}