using Bank.Accounts.Domain.Entities;

namespace Bank.Accounts.Application.Repositories;

public interface IAccountRepository
{
    Task AddAsync(Account account);
    Task<List<Account>> GetAllAsync(int pageNumber, int pageSize);
    Task<Account?> GetByAccountNumber(int accountNumber);
    Task<List<Account>> GetByIdOrAccountNumber(Guid accountId, int accountNumber);
    Task DeleteAsync(Guid accountId);
}