using Bank.Accounts.Application.Models;
using Bank.Commons.Applications.Factories.Results;

namespace Bank.Accounts.Application.Gateways;

public interface IBankTransactionsClient
{
    Task<Result<List<AccountBalanceApplication>>> GetAccountsBalanceAsync(List<Guid> accountIds);
    Task<Result<TransferApplication>> MakeTransferAsync(TransferApplication transfer);
}