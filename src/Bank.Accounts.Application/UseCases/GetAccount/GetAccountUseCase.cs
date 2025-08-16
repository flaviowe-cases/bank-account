using Bank.Accounts.Application.Factories.Results;
using Bank.Accounts.Application.Mappers;
using Bank.Accounts.Application.Repositories;
using Bank.Accounts.Application.Services;
using Bank.Accounts.Application.Services.Amounts;
using Bank.Accounts.Application.UseCases.AddAccount;
using FluentValidation;

namespace Bank.Accounts.Application.UseCases.GetAccount;

public class GetAccountUseCase(
    IValidator<GetAccountInput> validator, 
    IAccountRepository accountRepository,
    IResultFactory resultFactory,
    IAccountApplicationMapper  accountApplicationMapper,
    IAmountService amountService) : IGetAccountUseCase
{
    private readonly IValidator<GetAccountInput> _validator = validator;
    private readonly IAccountRepository _accountRepository = accountRepository;
    private readonly IResultFactory _resultFactory = resultFactory;
    private readonly IAmountService _amountService = amountService;

    public async Task<Result<GetAccountOutput>> HandleAsync(GetAccountInput input)
    {
        var validation = await _validator.ValidateAsync(input);

        if (!validation.IsValid)
            return _resultFactory.CreateFailure<GetAccountOutput>(
                "INVALID_FIELDS",
                "Invalid fields",
                validation.Errors);
        
        var account = await _accountRepository
            .GetByIdAsync(input.AccountId);

        if (account is null)
            return _resultFactory.CreateFailure<GetAccountOutput>(
                "ACCOUNT_NOT_FOUND", "Account not found");

        var accountApplication = accountApplicationMapper
            .ToApplication(account);

        accountApplication = await _amountService
            .LoadAmountAsync(accountApplication);

        if (accountApplication == null)
            return _resultFactory.CreateFailure<GetAccountOutput>(
                "BALANCE_TEMPORARILY_UNAVAILABLE",
                "balance temporarily unavailable");

        var output = new GetAccountOutput()
        {
            Account = accountApplication
        };
        
        return _resultFactory.CreateSuccess(output);    
    }
}