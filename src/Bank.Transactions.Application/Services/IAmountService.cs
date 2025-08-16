namespace Bank.Transactions.Application.Services;

public interface IAmountService
{
    Task<decimal> GetCurrentBalanceAsync(Guid accountId);
}