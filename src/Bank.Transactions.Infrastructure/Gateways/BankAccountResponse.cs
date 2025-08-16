using Bank.Transactions.Application.Models;

namespace Bank.Transactions.Infrastructure.Gateways;

public class BankAccountResponse
{
    public required IEnumerable<BankAccount> Accounts { get; set; }
}