namespace Bank.Transactions.Domain.Entities;

public class Transaction
{
    public Guid Id { get; set; }
    public required Guid SourceAccountId { get; init; }
    public required Guid DestinationAccountId { get; init; }
    public required decimal Amount { get; init; }
    public required TransactionStatusType Status { get; set; }
    public string? Comments { get; init; }
    public required DateTime TimeStamp { get; init; }
}