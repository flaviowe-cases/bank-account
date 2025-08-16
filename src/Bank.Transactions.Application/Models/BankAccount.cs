namespace Bank.Transactions.Application.Models;

public class BankAccount
{
    public required Guid Id { get; init; }
    public required string Name { get; init; }
    public required int Number { get; init; }
}