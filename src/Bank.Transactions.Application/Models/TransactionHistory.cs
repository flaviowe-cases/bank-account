using Bank.Transactions.Domain.Entities;

namespace Bank.Transactions.Application.Models;

public class TransactionHistory
{
    public DateTime TimeStamp { get; init; }
    public required Guid TransactionId { get; init; }
    public required string Status { get; set; }
    public required string TransactionType { get; init; }
    public required string OperationType { get; init; }
    public required decimal Amount { get; init; }
    public int? AccountSource { get; set; } 
    public int? AccountDestination { get; set; }
    public string? Comments { get; set; }
}