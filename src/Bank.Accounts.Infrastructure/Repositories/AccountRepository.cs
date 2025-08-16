using Bank.Accounts.Application.Repositories;
using Bank.Accounts.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Bank.Accounts.Infrastructure.Repositories;

public class AccountRepository(
    AccountContext accountContext) : IAccountRepository
{
    private readonly AccountContext _accountContext = accountContext;

    public async Task AddAsync(Account account)
    {
        await _accountContext
            .Accounts
            .AddAsync(account);

        await _accountContext
            .SaveChangesAsync();
    }

    public async Task<List<Account>> GetAllAsync(int pageNumber, int pageSize)
    {
        var query = from account in _accountContext.Accounts
                    select account;
        
        query = query
            .Skip(pageSize * (pageNumber - 1))
            .Take(pageSize);
        
        return await query.ToListAsync();
    }

    public async Task<Account?> GetByAccountNumber(int accountNumber)
    {
        var query = from account in _accountContext.Accounts
                    where  account.Number == accountNumber
                    select account; 
        
        return await query
            .FirstOrDefaultAsync(); 
    }

    public async Task<List<Account>> GetByIdOrAccountNumber(Guid accountId, int accountNumber)
    {
        var query = from account in _accountContext.Accounts
                    where account.Id == accountId ||
                          account.Number == accountNumber
                    select account;
        
        return await query
            .ToListAsync();
    }

    public async Task DeleteAsync(Guid accountId)
    {
        var account = await _accountContext.Accounts
            .FirstOrDefaultAsync(account => account.Id == accountId);
        
        if (account == null)
            return;
        
        _accountContext.Remove(account);
        await _accountContext.SaveChangesAsync();
    }
}