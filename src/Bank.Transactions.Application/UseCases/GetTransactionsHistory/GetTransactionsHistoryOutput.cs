using Bank.Transactions.Application.Models;

namespace Bank.Transactions.Application.UseCases.GetTransactionsHistory;

public class GetTransactionsHistoryOutput
{
    public required List<TransactionHistory> History { get; set; }  
}