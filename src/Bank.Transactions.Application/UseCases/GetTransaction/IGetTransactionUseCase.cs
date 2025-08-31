using Bank.Commons.Applications.Factories.Results;

namespace Bank.Transactions.Application.UseCases.GetTransaction;

public interface IGetTransactionUseCase
{
    Task<Result<GetTransactionOutput>> HandleAsync(GetTransactionInput input);
}