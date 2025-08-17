using Bank.Transactions.Application.Factories.Results;
using Bank.Transactions.Application.Models;

namespace Bank.Transactions.Application.Gateways;

public interface IBankAccountClient
{
    Task<Result<BankAccount>> GetByAccountAsync(int accountNumber);
    Task<Result<BankAccount>> GetByAccountAsync(Guid accountId);

}