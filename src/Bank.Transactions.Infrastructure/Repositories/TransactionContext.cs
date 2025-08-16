using Bank.Transactions.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Bank.Transactions.Infrastructure.Repositories;

public class TransactionContext(DbContextOptions<TransactionContext> options) : DbContext(options)
{
    public DbSet<Transaction> Transactions { get; set; }
}