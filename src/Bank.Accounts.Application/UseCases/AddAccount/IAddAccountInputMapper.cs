using Bank.Accounts.Domain.Entities;

namespace Bank.Accounts.Application.UseCases.AddAccount;

public interface IAddAccountInputMapper
{
    Account ToDomain(AddAccountInput addAccountInput);
}