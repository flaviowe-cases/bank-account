using Bank.Commons.Applications.Factories.Results;

namespace Bank.Accounts.Application.UseCases.AddAccount;

public interface IAddAccountUseCase
{
    Task<Result<AddAccountOutput>> HandleAsync(AddAccountInput input);
}