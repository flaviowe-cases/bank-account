namespace Bank.Transactions.Application.UseCases.GetTransactionsBalance;

public class GetTransactionsBalanceInput
{
    public required Guid[]  AccountsId { get; init; }
}