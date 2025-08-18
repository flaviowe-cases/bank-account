using Bank.Transactions.Domain.Entities;

namespace Bank.Transactions.Application.Repositories;

public interface ITransactionRepository
{
    Task AddAsync(Transaction transaction);
    Task<List<Transaction>> GetByAccountIdAsync(Guid accountId, TransactionStatusType? status = null);
}