using Bank.Transactions.Domain.Entities;

namespace Bank.Transactions.Application.Services;

public interface ITransactionService
{
    Task<Transaction> ExecuteAsync(Transaction transaction);
}