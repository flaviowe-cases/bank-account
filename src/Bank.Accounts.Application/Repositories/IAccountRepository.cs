using Bank.Accounts.Domain.Entities;

namespace Bank.Accounts.Application.Repositories;

public interface IAccountRepository
{
    Task AddAsync(Transaction transaction);
    Task DeleteAsync(Guid accountId);
    Task<Transaction?> GetByIdAsync(Guid accountId);
    Task<Transaction?> GetByNumberAsync(int accountNumber);
    Task<List<Transaction>> GetByIdOrAccountNumberAsync(Guid accountId, int accountNumber);
    Task<List<Transaction>> GetAllAsync(int pageNumber, int pageSize);
}