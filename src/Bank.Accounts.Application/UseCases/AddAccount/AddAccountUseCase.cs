using Bank.Accounts.Domain.Entities;
using FluentValidation;

namespace Bank.Accounts.Application.UseCases.AddAccount;

using Factories.Results;
using Repositories;

public class AddAccountUseCase(
    IValidator<AddAccountInput> validator,
    IAccountRepository accountRepository,
    IResultFactory resultFactory,
    IAddAccountInputMapper addAccountInputMapper) : IAddAccountUseCase
{
    private readonly IValidator<AddAccountInput> _validator = validator;
    private readonly IAccountRepository _accountRepository = accountRepository;
    private readonly IResultFactory _resultFactory = resultFactory;
    private readonly IAddAccountInputMapper _addAccountInputMapper = addAccountInputMapper;

    public async Task<Result<AddAccountOutput>> HandleAsync(AddAccountInput input)
    {
        var validation = await _validator.ValidateAsync(input);

        if (!validation.IsValid)
            return _resultFactory.CreateFailure<AddAccountOutput>(
                "INVALID_FIELDS",
                "Invalid fields",
                validation.Errors);

        var existingAccounts = await _accountRepository
            .GetByIdOrAccountNumber(input.AccountId, input.AccountNumber);

        if (existingAccounts.Count != 0)
            return CreateAlreadyExists(input, existingAccounts);

        var account = _addAccountInputMapper.ToDomain(input);

        await _accountRepository.AddAsync(account);

        return _resultFactory.CreateSuccess(new AddAccountOutput()
        {
            AccountId = account.Id
        });
    }

    private Result<AddAccountOutput> CreateAlreadyExists(
        AddAccountInput input, List<Account> existingAccounts)
    {
        var failures = new List<ResultFail>();

        if (existingAccounts.Any(account => account.Id == input.AccountId))
            failures.Add(new ResultFail
            {
                Code = "ACCOUNT_ID_ALREADY_EXISTS",
                Message = "Account id already exists"
            });

        if (existingAccounts.Any(account => account.Number == input.AccountNumber))
            failures.Add(new ResultFail
            {
                Code = "ACCOUNT_NUMBER_ALREADY_EXISTS",
                Message = "Account id already exists"
            });

        return _resultFactory.CreateFailure<AddAccountOutput>(failures);
    }
}