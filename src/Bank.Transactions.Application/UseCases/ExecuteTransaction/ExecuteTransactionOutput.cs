using Bank.Transactions.Domain.Entities;

namespace Bank.Transactions.Application.UseCases.ExecuteTransaction;

public class ExecuteTransactionOutput
{
    public required Transaction Transaction { get; set; }
}