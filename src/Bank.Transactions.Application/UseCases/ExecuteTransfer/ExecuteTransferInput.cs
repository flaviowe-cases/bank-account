namespace Bank.Transactions.Application.UseCases.ExecuteTransfer;

public class ExecuteTransferInput()
{
    public required int SourceAccountNumber { get; set; }
    public required int DestinationAccountNumber { get; set; }
    public required decimal Amount { get; set; }
    public string? Comments { get; set; }
}