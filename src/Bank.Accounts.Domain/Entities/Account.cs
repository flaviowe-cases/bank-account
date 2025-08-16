namespace Bank.Accounts.Domain.Entities;

public class Account
{
    public required Guid Id { get; init; }
    public required string Name { get; init; }
    public required int Number { get; init; }
    public required decimal Balance { get; init; } 
}