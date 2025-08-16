using Bank.Accounts.Application.Gateways;
using Bank.Accounts.Application.Models;

namespace Bank.Accounts.Application.Services.Amounts;

public class AmountService(
    IBankTransactionsClient bankTransactionsClient) : IAmountService
{
    private readonly IBankTransactionsClient _bankTransactionsClient = bankTransactionsClient;
    
    public async Task<List<AccountApplication>?> LoadAmountsAsync(List<AccountApplication> accounts)
    {
        var amountsId =  accounts
            .Select(account => account.Id)
            .ToList();

        var result = await _bankTransactionsClient.GetAmountsAsync(amountsId);

        if (!result.Success)
            return null;

        var amounts = result.GetContent();
        
        foreach (var accountApplication in accounts)
            accountApplication.Amount =  amounts
                .FirstOrDefault(amount => amount.AccountId == accountApplication.Id)?
                .Amount;

        return accounts;
    }

    public async Task<AccountApplication?> LoadAmountAsync(AccountApplication account)
    {
        var accounts = await LoadAmountsAsync([account]);

        return accounts?
            .FirstOrDefault();
    }
}