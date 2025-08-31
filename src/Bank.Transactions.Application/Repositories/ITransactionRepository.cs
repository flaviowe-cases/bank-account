using Bank.Transactions.Domain.Entities;

namespace Bank.Transactions.Application.Repositories;

public interface ITransactionRepository
{
    Task<Transaction?> GetAsync(Guid transactionId);
    Task AddAsync(Transaction transaction);
    Task UpdateAsync(Transaction transaction);   
    Task<List<Transaction>> GetByAccountIdAsync(Guid accountId, TransactionStatusType? status = null);
}