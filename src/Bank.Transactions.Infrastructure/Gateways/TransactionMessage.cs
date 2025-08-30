using Bank.Transactions.Domain.Entities;

namespace Bank.Transactions.Infrastructure.Gateways;

public class TransactionMessage
{
    public Guid Id { get; set; }
    public required Transaction Transaction { get; init; }
}