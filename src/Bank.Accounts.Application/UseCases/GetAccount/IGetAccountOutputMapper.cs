using Bank.Accounts.Application.Models;

namespace Bank.Accounts.Application.UseCases.GetAccount;

public interface IGetAccountOutputMapper
{
    GetAccountOutput Map(AccountApplication accountApplication);
}