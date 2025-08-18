namespace Bank.Transactions.Domain.Entities;

public class TransferParameters
{
    public required decimal LimitAmountTransfer { get; init; }
}