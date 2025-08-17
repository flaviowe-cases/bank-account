using Bank.Accounts.Domain.Entities;

namespace Bank.Accounts.Application.UseCases.AddAccount;

public class AddAccountInputMapper : IAddAccountInputMapper
{
    public Transaction ToDomain(AddAccountInput addAccountInput)
    {
        return new Transaction
        {
            Id = Guid.NewGuid(),
            Number = addAccountInput.AccountNumber,
            Name = addAccountInput.Name,
        };
    }
}