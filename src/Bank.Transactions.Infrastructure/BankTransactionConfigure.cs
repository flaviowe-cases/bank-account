namespace Bank.Transactions.Infrastructure;

public class BankTransactionConfigure
{
    public required string BankAccountBaseAddress { get; init; }
    public required decimal LimitAmountTransfer { get; init; }
    public required string TransactionConnectionString { get; init; }
    public required string TransactionDatabaseName { get; init; }
    public required string MessageQueueHost { get; init; }
    public required string MessageGroupId { get; init; }
}