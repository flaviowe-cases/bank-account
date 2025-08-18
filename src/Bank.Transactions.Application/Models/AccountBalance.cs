namespace Bank.Transactions.Application.Models;

public class AccountBalance
{
    public required Guid AccountId { get; set; }
    public required decimal Amount { get; set; } 
}