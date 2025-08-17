using Bank.Commons.Applications.Factories.Results;

namespace Bank.Accounts.Application.UseCases.GetAccount;

public interface IGetAccountUseCase
{
    Task<Result<GetAccountOutput>> HandleAsync(GetAccountInput input);
}