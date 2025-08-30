using Bank.Transactions.Domain.Entities;

namespace Bank.Transactions.Application.UseCases.GetTransaction;

public class GetTransactionOutput
{
    public required Transaction Transaction { get; set; }
}