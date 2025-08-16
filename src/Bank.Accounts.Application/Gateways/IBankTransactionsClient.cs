using Bank.Accounts.Application.Factories.Results;
using Bank.Accounts.Application.Models;

namespace Bank.Accounts.Application.Gateways;

public interface IBankTransactionsClient
{
    Task<Result<List<AmountApplication>>> GetAmountsAsync(List<Guid> accountIds);
    Task<Result<TransferApplication>> MakeTransferAsync(TransferApplication transfer);
}