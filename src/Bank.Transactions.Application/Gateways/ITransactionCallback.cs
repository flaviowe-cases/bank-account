using Bank.Transactions.Domain.Entities;

namespace Bank.Transactions.Application.Gateways;

public interface ITransactionCallback
{
    Task SendAsync(Transaction transaction);
    Task<Transaction> CallbackAsync(Guid transactionId);
}