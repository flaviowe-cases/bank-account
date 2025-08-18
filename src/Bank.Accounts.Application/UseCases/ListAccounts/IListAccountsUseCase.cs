using Bank.Commons.Applications.Factories.Results;

namespace Bank.Accounts.Application.UseCases.ListAccounts;

public interface IListAccountsUseCase
{
    Task<Result<ListAccountsOutput>> HandleAsync(ListAccountsInput input);
}