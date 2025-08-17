using Bank.Accounts.Application.Models;
using Bank.Accounts.Domain.Entities;

namespace Bank.Accounts.Application.Mappers;

public class AccountApplicationMapper : IAccountApplicationMapper
{
    public List<AccountApplication> ToApplication(List<Transaction> accounts)
        => accounts
            .Select(ToApplication)
            .ToList();  

    public AccountApplication ToApplication(Transaction transaction)
    {
        return new AccountApplication
        {
            Id = transaction.Id,
            Name = transaction.Name,
            Number = transaction.Number,
            Amount = 0
        };
    }
}