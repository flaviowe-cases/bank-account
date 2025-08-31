using Bank.Commons.Applications.Factories.Results;

namespace Bank.Transactions.Application.UseCases.CreateTransaction;

public interface ICreateTransactionUseCase
{
    Task<Result<CreateTransactionOutput>> HandleAsync(
        CreateTransactionInput input);
}