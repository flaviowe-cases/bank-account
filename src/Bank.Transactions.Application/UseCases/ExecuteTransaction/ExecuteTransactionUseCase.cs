using Bank.Commons.Applications.Factories.Results;
using Bank.Transactions.Application.Services;

namespace Bank.Transactions.Application.UseCases.ExecuteTransaction;

public class ExecuteTransactionUseCase(
    ITransactionService transactionService,
    IResultFactory resultFactory) : IExecuteTransactionUseCase
{
    private readonly ITransactionService _transactionService = transactionService;
    private readonly IResultFactory _resultFactory = resultFactory;

    public async Task<Result<ExecuteTransactionOutput>> HandleAsync(
        ExecuteTransactionInput input)
    {
        var transaction = await _transactionService.ExecuteAsync(input.Transaction);

        return _resultFactory.CreateSuccess(new ExecuteTransactionOutput()
        {
            Transaction = transaction,
        });
    }
}