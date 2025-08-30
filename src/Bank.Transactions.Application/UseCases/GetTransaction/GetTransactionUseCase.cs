using Bank.Commons.Applications.Factories.Results;
using Bank.Transactions.Application.Repositories;

namespace Bank.Transactions.Application.UseCases.GetTransaction;

public class GetTransactionUseCase(
    ITransactionRepository  transactionRepository,
    IResultFactory resultFactory) : IGetTransactionUseCase
{
    private readonly ITransactionRepository _transactionRepository = transactionRepository;
    private readonly IResultFactory _resultFactory = resultFactory;

    public async Task<Result<GetTransactionOutput>> HandleAsync(GetTransactionInput input)
    {
        var transaction = await _transactionRepository
            .GetAsync(input.TransactionId);
        
        if  (transaction == null)
            return _resultFactory.CreateFailure<GetTransactionOutput>(
                "TRANSACTION_NOT_FOUND", "Transaction not found");

        var output = new GetTransactionOutput()
        {
            Transaction = transaction
        };
        
        return _resultFactory.CreateSuccess(output);
    }
}