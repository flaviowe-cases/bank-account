using Bank.Accounts.Application.Models;

namespace Bank.Accounts.Application.UseCases.ListAccounts;

public class ListAccountsOutput
{
    public required IEnumerable<AccountApplication> Accounts { get; set; } 
}