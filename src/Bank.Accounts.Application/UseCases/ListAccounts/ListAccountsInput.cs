namespace Bank.Accounts.Application.UseCases.ListAccounts;

public class ListAccountsInput
{
    public required int PageNumber { get; init; }   
    public required int PageSize { get; init; }
}