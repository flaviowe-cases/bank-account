namespace Bank.Accounts.Application.UseCases.AddAccount;

public class AddAccountInput
{
    public required Guid AccountId { get; set; }
    public required string Name { get; set; }
    public required int AccountNumber { get; set; }
    public decimal Amount { get; set; }
}