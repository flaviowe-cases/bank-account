using Bank.Transactions.Application.Factories.Results;

namespace Bank.Transactions.Application.UseCases.GetTransactionsHistory;

public interface IGetTransactionsHistoryUseCase
{
    Task<Result<GetTransactionsHistoryOutput>> HandleAsync(
        GetTransactionsHistoryInput input);
}