namespace Bank.Accounts.Api.Controllers.AddAccount;

public class AddAccountRequest
{
    public Guid? AccountId { get; set; }
    public string? Name { get; set; }
    public int AccountNumber { get; set; }
    public decimal InitialAmount { get; set; }
    
}