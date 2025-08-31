using Bank.Transactions.Domain.Entities;

namespace Bank.Transactions.Application.UseCases.CreateTransaction;

public class CreateTransactionInputMapper : ICreateTransactionInputMapper
{
    public Transaction MapToDomain(
        CreateTransactionInput input,
        Guid sourceAccountId, 
        Guid destinationAccountId)
    {
        return new Transaction
        {
            Id = Guid.NewGuid(),
            SourceAccountId = sourceAccountId,
            DestinationAccountId = destinationAccountId,
            Amount = input.Amount,
            Status = TransactionStatusType.Pending,
            Comments = input.Comments,
            TimeStamp = DateTime.UtcNow
        };
    }
}