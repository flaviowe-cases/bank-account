namespace Bank.Accounts.Application.Models;

public class AccountBalanceApplication
{
    public required Guid AccountId { get; set; }
    public required decimal Amount { get; set; } 
}