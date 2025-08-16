using Bank.Accounts.Application.Gateways;
using Bank.Accounts.Application.Models;
using Bank.Accounts.Domain.Entities;
using Microsoft.Extensions.Logging;

namespace Bank.Accounts.Application.Services.Amounts;

public class AmountService(
    ILogger<AmountService> logger,  
    IBankTransactionsClient bankTransactionsClient) : IAmountService
{
    private readonly ILogger<AmountService> _logger = logger;
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

    public async Task<bool> MakeTransferAsync(Account account, decimal amount, string comments)
    {
        var transfer = new TransferApplication()
        {
            SourceAccountNumber = 0,
            DestinationAccountNumber = account.Number,
            Amount = amount,
            Comments = comments,
        };
        
        var result = await _bankTransactionsClient.MakeTransferAsync(transfer);
        
        if (!result.Success)
            _logger.LogCritical("Fail to make transfer {Id} {Reason}",
                account.Id, result.Failures?.FirstOrDefault()?.Message);
        
        return result.Success;
    }
}