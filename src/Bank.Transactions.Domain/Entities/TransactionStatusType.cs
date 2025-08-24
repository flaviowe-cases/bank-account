namespace Bank.Transactions.Domain.Entities;

public enum TransactionStatusType
{
    Pending = 0,
    Success = 1,
    InsufficientFunds = 2,
    LimitExceeded = 3,
}