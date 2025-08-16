using Bank.Accounts.Application.Factories.Results;
using Bank.Accounts.Application.Mappers;
using Bank.Accounts.Application.Repositories;
using Bank.Accounts.Application.Services;
using Bank.Accounts.Application.Services.Amounts;
using FluentValidation;

namespace Bank.Accounts.Application.UseCases.ListAccounts;

public class ListAccountsUseCase(
    IValidator<ListAccountsInput> validator,   
    IAccountRepository  accountRepository,
    IResultFactory  resultFactory,
    IAmountService  amountService,
    IAccountApplicationMapper accountApplicationMapper) : IListAccountsUseCase
{
    private readonly IValidator<ListAccountsInput> _validator = validator;
    private readonly IAccountRepository _accountRepository = accountRepository;
    private readonly IResultFactory _resultFactory = resultFactory;
    private readonly IAmountService _amountService = amountService;
    private readonly IAccountApplicationMapper _accountApplicationMapper = accountApplicationMapper;

    public async Task<Result<ListAccountsOutput>> HandleAsync(ListAccountsInput input)
    {
        var validation = await _validator.ValidateAsync(input);

        if (!validation.IsValid)
            return _resultFactory.CreateFailure<ListAccountsOutput>(
                "INVALID_FIELDS",
                "Invalid fields",
                validation.Errors);
        
        var accounts = await _accountRepository
            .GetAllAsync(input.PageNumber, input.PageSize);
        
        if (accounts.Count == 0)
            return _resultFactory.CreateSuccess(new ListAccountsOutput()
            {
                Accounts = []
            });
        
        var accountApplications = _accountApplicationMapper
            .ToApplication(accounts);

        var loadAmount = !input.IgnoreAmount ?? false;

        if (loadAmount)
        {
            accountApplications = await _amountService
                .LoadAmountsAsync(accountApplications);
            
            if (accountApplications == null)
                return _resultFactory.CreateFailure<ListAccountsOutput>(
                    "BALANCE_TEMPORARILY_UNAVAILABLE",
                    "balance temporarily unavailable");
        }

        var output = new ListAccountsOutput()
        {
            Accounts = accountApplications
        };

        return _resultFactory.CreateSuccess(output);
    }
}