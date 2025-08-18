using Bank.Accounts.Application.Models;

namespace Bank.Accounts.Application.UseCases.GetAccount;

public class GetAccountOutputMapper : IGetAccountOutputMapper
{
    public GetAccountOutput Map(AccountApplication accountApplication)
        => new GetAccountOutput() {Account = accountApplication};
}