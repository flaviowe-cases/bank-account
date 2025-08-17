using Bank.Accounts.Application.Factories.Results;
using Bank.Accounts.Application.Mappers;
using Bank.Accounts.Application.Repositories;
using Bank.Accounts.Application.Services.Amounts;
using Bank.Accounts.Domain.Entities;
using FluentValidation;

namespace Bank.Accounts.Application.UseCases.GetAccount;

public class GetAccountUseCase(
    IValidator<GetAccountInput> validator, 
    IAccountRepository accountRepository,
    IAccountApplicationMapper accountApplicationMapper,
    IGetAccountOutputMapper accountOutputMapper,
    IAmountService amountService,
    IResultFactory resultFactory) : IGetAccountUseCase
{
    private readonly IValidator<GetAccountInput> _validator = validator;
    private readonly IAccountRepository _accountRepository = accountRepository;
    private readonly IAccountApplicationMapper _accountApplicationMapper = accountApplicationMapper;
    private readonly IGetAccountOutputMapper _accountOutputMapper = accountOutputMapper;
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

        Account? account = null;
        
        if (input.AccountId.HasValue)
            account = await _accountRepository.GetByIdAsync(input.AccountId.Value);

        else if(input.AccountNumber.HasValue)
            account = await _accountRepository.GetByNumberAsync(input.AccountNumber.Value);
        
        if (account is null)
            return _resultFactory.CreateFailure<GetAccountOutput>(
                "ACCOUNT_NOT_FOUND", "Account not found");

        var accountApplication = _accountApplicationMapper
            .ToApplication(account);

        accountApplication = await _amountService
            .LoadAmountAsync(accountApplication);

        if (accountApplication == null)
            return _resultFactory.CreateFailure<GetAccountOutput>(
                "BALANCE_TEMPORARILY_UNAVAILABLE",
                "balance temporarily unavailable");

        var output = _accountOutputMapper.Map(accountApplication);
        
        return _resultFactory.CreateSuccess(output);    
    }
}