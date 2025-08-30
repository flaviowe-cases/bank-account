using Bank.Transactions.Application.Models;
using Bank.Transactions.Domain.Entities;

namespace Bank.Transactions.Application.Gateways;

public interface ITransactionProducer
{
    Task ExecuteTransactionAsync(Transaction transaction);
}