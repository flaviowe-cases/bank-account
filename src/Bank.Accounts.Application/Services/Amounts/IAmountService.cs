using Bank.Accounts.Application.Models;
using Bank.Accounts.Domain.Entities;

namespace Bank.Accounts.Application.Services.Amounts;

public interface IAmountService
{
     Task<List<AccountApplication>?> LoadAmountsAsync(List<AccountApplication> accounts);
     Task<AccountApplication?> LoadAmountAsync(AccountApplication account);
     Task<bool> MakeTransferAsync(Account account, decimal amount, string openAccount);
}