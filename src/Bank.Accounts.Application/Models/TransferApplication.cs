namespace Bank.Accounts.Application.Models;

public class TransferApplication
{
    public int? SourceAccountNumber { get; set; }
    public int? DestinationAccountNumber { get; set; }
    public required decimal Amount { get; set; }
    public string? Comments { get; set; }
}