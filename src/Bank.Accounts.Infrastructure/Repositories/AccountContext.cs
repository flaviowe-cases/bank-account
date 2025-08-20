using Bank.Accounts.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Bank.Accounts.Infrastructure.Repositories;

public class AccountContext(DbContextOptions<AccountContext> options) : DbContext(options)
{
    public DbSet<Account> Accounts { get; set; }
}