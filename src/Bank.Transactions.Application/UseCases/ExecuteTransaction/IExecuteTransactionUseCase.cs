using Bank.Transactions.Application.Factories.Results;

namespace Bank.Transactions.Application.UseCases.ExecuteTransaction;

public interface IExecuteTransactionUseCase
{
    Task<Result<ExecuteTransactionOutput>> HandleAsync(ExecuteTransactionInput input);
}