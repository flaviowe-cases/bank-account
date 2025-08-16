using Bank.Transactions.Application.Repositories;
using Bank.Transactions.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Bank.Transactions.Infrastructure.Repositories;

public class TransactionRepository(
    TransactionContext transactionContext) : ITransactionRepository
{
    private readonly TransactionContext _transactionContext = transactionContext;

    public async Task AddAsync(Transaction transaction)
    {
        await _transactionContext.AddAsync(transaction);
        await _transactionContext.SaveChangesAsync();
    }

    public async Task<List<Transaction>> GetByAccountIdAsync(
        Guid accountId, TransactionStatusType? status = null)
    {
        var query = from transaction in _transactionContext.Transactions
                    where (transaction.SourceAccountId == accountId || 
                           transaction.DestinationAccountId == accountId)
                        select transaction;
        
        if (status != null)
            query = query.Where(transaction => transaction.Status == status);

        return await query.ToListAsync();
    }
}