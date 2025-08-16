using Bank.Accounts.Domain.Entities;

namespace Bank.Accounts.Application.UseCases.AddAccount;

public class AddAccountInputMapper : IAddAccountInputMapper
{
    public Account ToDomain(AddAccountInput addAccountInput)
    {
        return new Account
        {
            Id = Guid.NewGuid(),
            Number = addAccountInput.AccountNumber,
            Name = addAccountInput.Name,
        };
    }
}