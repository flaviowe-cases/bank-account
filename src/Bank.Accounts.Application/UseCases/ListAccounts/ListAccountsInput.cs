namespace Bank.Accounts.Application.UseCases.ListAccounts;

public class ListAccountsInput
{
    public int? AccountNumber { get; set; }
    public required int PageNumber { get; init; }   
    public required int PageSize { get; init; }
}