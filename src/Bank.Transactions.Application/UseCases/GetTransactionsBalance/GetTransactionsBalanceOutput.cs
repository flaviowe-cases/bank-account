using Bank.Transactions.Application.Models;

namespace Bank.Transactions.Application.UseCases.GetTransactionsBalance;

public class GetTransactionsBalanceOutput
{
    public required List<AccountBalance> AccountBalance { get; set; } 
}