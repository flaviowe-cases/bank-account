namespace Bank.Accounts.Application.UseCases.GetAccount;

public class GetAccountInput
{
    public Guid? AccountId { get; set; }
    public int? AccountNumber { get; set; }
}