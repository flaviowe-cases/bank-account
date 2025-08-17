using AutoFixture;
using Bank.Accounts.Application.Mappers;
using Bank.Accounts.Application.Models;
using Bank.Accounts.Application.Repositories;
using Bank.Accounts.Application.Services.Amounts;
using Bank.Accounts.Application.UseCases.GetAccount;
using Bank.Accounts.Domain.Entities;
using Bank.Commons.Applications.Factories.Results;
using FluentAssertions;
using FluentValidation;
using FluentValidation.Results;
using NSubstitute;

namespace Bank.Accounts.Unit.Test.Application.UseCases;

public class GetAccountUseCaseTests
{
    [Fact]
    public async Task HandleAsync_With_AccountId_Should_Returns_Accounts()
    {
        //Arrange
        var fixture = new Fixture();
        var validator = Substitute.For<IValidator<GetAccountInput>>();
        var accountRepository = Substitute.For<IAccountRepository>();
        var resultFactory = Substitute.For<IResultFactory>();
        var accountApplicationMapper = Substitute.For<IAccountApplicationMapper>();
        var getAccountOutputMapper = Substitute.For<IGetAccountOutputMapper>();
        var amountService = Substitute.For<IAmountService>();

        var useCase = new GetAccountUseCase(
            validator,
            accountRepository,
            accountApplicationMapper,
            getAccountOutputMapper,
            amountService,
            resultFactory);

        var input = new GetAccountInput()
        {
            AccountId = Guid.NewGuid(),
        };

        var validation = new ValidationResult();
        var account = fixture.Create<Transaction>();
        var accountApplication = fixture.Create<AccountApplication>();
        var output = fixture.Create<GetAccountOutput>();
        var resultOutput = new Result<GetAccountOutput>(output) { Success = true };

        validator
            .ValidateAsync(input)
            .Returns(validation);

        accountRepository
            .GetByIdAsync(input.AccountId.Value)
            .Returns(account);

        accountApplicationMapper
            .ToApplication(account)
            .Returns(accountApplication);

        amountService
            .LoadAmountAsync(accountApplication)
            .Returns(accountApplication);

        getAccountOutputMapper
            .Map(accountApplication)
            .Returns(output);

        resultFactory
            .CreateSuccess(output)
            .Returns(resultOutput);

        //Act
        var result = await useCase.HandleAsync(input);

        //Assert
        result.Should().NotBeNull();
        result.Success.Should().BeTrue();
        result.GetContent().Should().Be(output);
    }

    [Fact]
    public async Task HandleAsync_With_AccountNumber_Should_Returns_Accounts()
    {
        //Arrange
        var fixture = new Fixture();
        var validator = Substitute.For<IValidator<GetAccountInput>>();
        var accountRepository = Substitute.For<IAccountRepository>();
        var resultFactory = Substitute.For<IResultFactory>();
        var accountApplicationMapper = Substitute.For<IAccountApplicationMapper>();
        var getAccountOutputMapper = Substitute.For<IGetAccountOutputMapper>();
        var amountService = Substitute.For<IAmountService>();

        var useCase = new GetAccountUseCase(
            validator,
            accountRepository,
            accountApplicationMapper,
            getAccountOutputMapper,
            amountService,
            resultFactory);

        var input = new GetAccountInput()
        {
            AccountNumber = fixture.Create<int>(),
        };

        var validation = new ValidationResult();
        var account = fixture.Create<Transaction>();
        var accountApplication = fixture.Create<AccountApplication>();
        var output = fixture.Create<GetAccountOutput>();
        var resultOutput = new Result<GetAccountOutput>(output) { Success = true };

        validator
            .ValidateAsync(input)
            .Returns(validation);

        accountRepository
            .GetByNumberAsync(input.AccountNumber.Value)
            .Returns(account);

        accountApplicationMapper
            .ToApplication(account)
            .Returns(accountApplication);

        amountService
            .LoadAmountAsync(accountApplication)
            .Returns(accountApplication);

        getAccountOutputMapper
            .Map(accountApplication)
            .Returns(output);

        resultFactory
            .CreateSuccess(output)
            .Returns(resultOutput);

        //Act
        var result = await useCase.HandleAsync(input);

        //Assert
        result.Should().NotBeNull();
        result.Success.Should().BeTrue();
        result.GetContent().Should().Be(output);
    }

    [Fact]
    public async Task HandleAsync_Should_Returns_Accounts_Not_Found()
    {
        //Arrange
        var fixture = new Fixture();
        var validator = Substitute.For<IValidator<GetAccountInput>>();
        var accountRepository = Substitute.For<IAccountRepository>();
        var resultFactory = Substitute.For<IResultFactory>();
        var accountApplicationMapper = Substitute.For<IAccountApplicationMapper>();
        var getAccountOutputMapper = Substitute.For<IGetAccountOutputMapper>();
        var amountService = Substitute.For<IAmountService>();

        var useCase = new GetAccountUseCase(
            validator,
            accountRepository,
            accountApplicationMapper,
            getAccountOutputMapper,
            amountService,
            resultFactory);

        var input = new GetAccountInput()
        {
            AccountNumber = fixture.Create<int>(),
        };

        var validation = new ValidationResult();
        var resultOutput = new Result<GetAccountOutput>(null) { Success = false };

        validator
            .ValidateAsync(input)
            .Returns(validation);

        accountRepository
            .GetByNumberAsync(input.AccountNumber.Value)
            .Returns((Transaction?)null);

        resultFactory
            .CreateFailure<GetAccountOutput>(
            "ACCOUNT_NOT_FOUND", "Account not found")
            .Returns(resultOutput);

        //Act
        var result = await useCase.HandleAsync(input);

        //Assert
        result.Should().NotBeNull();
        result.Success.Should().BeFalse();
    }

    [Fact]
    public async Task HandleAsync_Should_Returns_Accounts_Balance_Temporarily_Unavailable()
    {
        //Arrange
        var fixture = new Fixture();
        var validator = Substitute.For<IValidator<GetAccountInput>>();
        var accountRepository = Substitute.For<IAccountRepository>();
        var resultFactory = Substitute.For<IResultFactory>();
        var accountApplicationMapper = Substitute.For<IAccountApplicationMapper>();
        var getAccountOutputMapper = Substitute.For<IGetAccountOutputMapper>();
        var amountService = Substitute.For<IAmountService>();

        var useCase = new GetAccountUseCase(
            validator,
            accountRepository,
            accountApplicationMapper,
            getAccountOutputMapper,
            amountService,
            resultFactory);

        var input = new GetAccountInput()
        {
            AccountNumber = fixture.Create<int>(),
        };

        var validation = new ValidationResult();
        var account = fixture.Create<Transaction>();
        var accountApplication = fixture.Create<AccountApplication>();
        var output = fixture.Create<GetAccountOutput>();
        var resultOutput = new Result<GetAccountOutput>(output) { Success = false };

        validator
            .ValidateAsync(input)
            .Returns(validation);

        accountRepository
            .GetByNumberAsync(input.AccountNumber.Value)
            .Returns(account);

        accountApplicationMapper
            .ToApplication(account)
            .Returns(accountApplication);

        amountService
            .LoadAmountAsync(accountApplication)
            .Returns((AccountApplication?)null);

        resultFactory
            .CreateFailure<GetAccountOutput>(
                "BALANCE_TEMPORARILY_UNAVAILABLE", "balance temporarily unavailable")
            .Returns(resultOutput);

        //Act
        var result = await useCase.HandleAsync(input);

        //Assert
        result.Should().NotBeNull();
        result.Success.Should().BeFalse();
    }
}