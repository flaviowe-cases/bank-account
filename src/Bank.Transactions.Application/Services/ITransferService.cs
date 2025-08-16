using Bank.Transactions.Application.Models;
using Bank.Transactions.Domain.Entities;

namespace Bank.Transactions.Application.Services;

public interface ITransferService
{
    Task<Transaction> ExecuteAsync(
        Guid sourceAccountId,
        Guid destinationAccountId,
        decimal amount, 
        string? comments);
}