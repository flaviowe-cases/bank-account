namespace Bank.Accounts.Domain.Entities;

public class Transaction
{
    public required Guid Id { get; init; }
    public required string Name { get; init; }
    public required int Number { get; init; }
}