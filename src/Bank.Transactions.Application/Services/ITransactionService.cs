using Bank.Transactions.Domain.Entities;

namespace Bank.Transactions.Application.Services;

public interface ITransactionService
{
    Task<Transaction> ExecuteAsync(
        Guid sourceAccountId,
        Guid destinationAccountId,
        decimal amount, 
        string? comments);
}