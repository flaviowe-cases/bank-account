using Bank.Accounts.Application.Models;

namespace Bank.Accounts.Infrastructure.Gateways;

public class TransactionBalanceResponse
{
    public required IEnumerable<AmountApplication> AccountBalance { get; set; }    
}