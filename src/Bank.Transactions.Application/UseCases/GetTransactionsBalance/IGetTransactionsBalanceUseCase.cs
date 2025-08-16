using Bank.Transactions.Application.Factories.Results;

namespace Bank.Transactions.Application.UseCases.GetTransactionsBalance;

public interface IGetTransactionsBalanceUseCase
{
    Task<Result<GetTransactionsBalanceOutput>> HandleAsync(
        GetTransactionsBalanceInput input);
}