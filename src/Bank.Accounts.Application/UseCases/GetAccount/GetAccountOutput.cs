using Bank.Accounts.Application.Models;

namespace Bank.Accounts.Application.UseCases.GetAccount;

public class GetAccountOutput
{
    public required AccountApplication Account { get; set; } 
}