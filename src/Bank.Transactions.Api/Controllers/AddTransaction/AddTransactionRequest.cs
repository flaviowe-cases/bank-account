namespace Bank.Transactions.Api.Controllers.AddTransaction;

public class AddTransactionRequest
{
    public int? SourceAccountNumber { get; init; }
    public int? DestinationAccountNumber { get; init; }
    public required decimal Amount { get; init; }
    public string? Comments { get; init; }
}