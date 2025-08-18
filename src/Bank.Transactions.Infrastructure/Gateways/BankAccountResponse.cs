using Bank.Transactions.Application.Models;

namespace Bank.Transactions.Infrastructure.Gateways;

public class BankAccountResponse
{
    public required BankAccount Account { get; set; }
}