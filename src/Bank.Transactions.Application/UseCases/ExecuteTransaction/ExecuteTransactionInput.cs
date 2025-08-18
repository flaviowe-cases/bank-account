namespace Bank.Transactions.Application.UseCases.ExecuteTransaction;

public class ExecuteTransactionInput()
{
    public required int? SourceAccountNumber { get; init; }
    public required int? DestinationAccountNumber { get; init; }
    public required decimal Amount { get; init; }
    public string? Comments { get; init; }
}