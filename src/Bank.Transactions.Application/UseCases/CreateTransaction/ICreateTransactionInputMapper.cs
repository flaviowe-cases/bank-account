
using Bank.Transactions.Domain.Entities;

namespace Bank.Transactions.Application.UseCases.CreateTransaction;

public interface ICreateTransactionInputMapper
{
    Transaction MapToDomain(
        CreateTransactionInput input, 
        Guid sourceAccountId, 
        Guid destinationAccountId);
}