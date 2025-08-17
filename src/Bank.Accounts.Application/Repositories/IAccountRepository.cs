using Bank.Accounts.Domain.Entities;

namespace Bank.Accounts.Application.Repositories;

public interface IAccountRepository
{
    Task AddAsync(Account account);
    Task DeleteAsync(Guid accountId);
    Task<Account?> GetByIdAsync(Guid accountId);
    Task<Account?> GetByNumberAsync(int accountNumber);
    Task<List<Account>> GetByIdOrAccountNumberAsync(Guid accountId, int accountNumber);
    Task<List<Account>> GetAllAsync(int pageNumber, int pageSize);
}