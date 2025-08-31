using Bank.Accounts.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Bank.Accounts.Infrastructure.Repositories;

public class AccountContext(DbContextOptions<AccountContext> options) 
    : DbContext(options)
{
    public DbSet<Account> Accounts { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Account>(entity =>
        {
            entity.ToTable("accounts");
            
            entity.HasKey(account => account.Id);

            entity.Property(account => account.Id)
                .HasColumnName("id");
            
            entity.Property(account => account.Name)
                .HasColumnName("name");
            
            entity.Property(account => account.Number)
                .HasColumnName("number");
        });

    }
}