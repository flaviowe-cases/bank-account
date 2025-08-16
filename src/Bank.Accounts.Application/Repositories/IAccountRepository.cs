using Bank.Accounts.Domain.Entities;

namespace Bank.Accounts.Application.Repositories;

public interface IAccountRepository
{
    Task<Account?> GetByIdAsync(Guid accountId);
    Task AddAsync(Account account);
    Task<List<Account>> GetAllAsync(int? accountNumber, int pageNumber, int pageSize);
    Task<List<Account>> GetByIdOrAccountNumberAsync(Guid accountId, int accountNumber);
    Task DeleteAsync(Guid accountId);

}