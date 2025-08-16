using Bank.Transactions.Application.Repositories;
using Bank.Transactions.Domain.Entities;

namespace Bank.Transactions.Application.Services;

public class AmountService(
    ITransactionRepository transactionRepository) : IAmountService
{
    private readonly ITransactionRepository _transactionRepository = transactionRepository;

    public async Task<decimal> GetCurrentBalanceAsync(Guid accountId)
    {
        var transactions = await _transactionRepository
            .GetByAccountIdAsync(accountId, TransactionStatusType.Success);
        
        var amountCredit = transactions
            .Where(transaction => transaction.DestinationAccountId == accountId)
            .Sum(transaction => transaction.Amount);

        var amountDebit = transactions
            .Where(transaction => transaction.DestinationAccountId == accountId)
            .Sum(transaction => transaction.Amount);

        return amountCredit - amountDebit;
    }
}