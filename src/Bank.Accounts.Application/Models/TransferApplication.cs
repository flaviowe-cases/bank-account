namespace Bank.Accounts.Application.Models;

public class TransferApplication
{
    public required int SourceAccountNumber { get; set; }
    public required int DestinationAccountNumber { get; set; }
    public required decimal Amount { get; set; }
    public string? Comments { get; set; }
}