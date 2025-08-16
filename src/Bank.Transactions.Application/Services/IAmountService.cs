using Bank.Transactions.Application.Models;

namespace Bank.Transactions.Application.Services;

public interface IAmountService
{
    Task<AccountBalance> GetCurrentBalanceAsync(Guid accountId);
}