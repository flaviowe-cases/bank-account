namespace Bank.Accounts.Application.Models;

public class AmountApplication
{
    public required Guid AccountId { get; set; }
    public required decimal Amount { get; set; } 
}