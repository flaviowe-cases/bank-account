using Bank.Accounts.Application.Models;
using Bank.Accounts.Domain.Entities;

namespace Bank.Accounts.Application.Mappers;

public class AccountApplicationMapper : IAccountApplicationMapper
{
    public List<AccountApplication> ToApplication(List<Account> accounts)
        => accounts
            .Select(ToApplication)
            .ToList();  

    public AccountApplication ToApplication(Account account)
    {
        return new AccountApplication
        {
            Id = account.Id,
            Name = account.Name,
            Number = account.Number,
            Amount = 0
        };
    }
}