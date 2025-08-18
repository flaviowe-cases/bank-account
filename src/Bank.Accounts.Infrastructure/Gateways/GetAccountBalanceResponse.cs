using Bank.Accounts.Application.Models;

namespace Bank.Accounts.Infrastructure.Gateways;

public class GetAccountBalanceResponse
{
    public required List<AccountBalanceApplication> AccountBalance { get; set; }    
}