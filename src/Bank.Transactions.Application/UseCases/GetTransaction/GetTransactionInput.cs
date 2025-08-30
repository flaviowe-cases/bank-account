namespace Bank.Transactions.Application.UseCases.GetTransaction;

public class GetTransactionInput
{
    public required Guid TransactionId { get; set; }
}