using Bank.Accounts.Domain.Entities;

namespace Bank.Accounts.Application.UseCases.AddAccount;

public interface IAddAccountInputMapper
{
    Transaction ToDomain(AddAccountInput addAccountInput);
}