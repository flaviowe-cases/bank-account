namespace Bank.Accounts.Application.Models;

public class AccountApplication
{
    public required Guid Id { get; init; }
    public required string Name { get; set; }
    public required int Number { get; set; }
    public decimal? Amount { get; set; }
}