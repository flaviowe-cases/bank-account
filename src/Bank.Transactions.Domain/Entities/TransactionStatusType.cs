namespace Bank.Transactions.Domain.Entities;

public enum TransactionStatusType
{
    Success,
    InsufficientFunds,
    LimitExceeded,
}