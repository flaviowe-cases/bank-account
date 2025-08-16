using Bank.Accounts.Application.Models;

namespace Bank.Accounts.Application.Services.Amounts;

public interface IAmountService
{
     Task<List<AccountApplication>?> LoadAmountsAsync(List<AccountApplication> accounts);
     Task<AccountApplication?> LoadAmountAsync(AccountApplication account);
}