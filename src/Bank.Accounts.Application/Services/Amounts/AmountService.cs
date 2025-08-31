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
    
    public async Task<AccountApplication?> LoadAmountAsync(AccountApplication account)
    {
        var accounts = await LoadAmountsAsync([account]);

        return accounts?
            .FirstOrDefault();
    }

    public async Task<List<AccountApplication>?> LoadAmountsAsync(List<AccountApplication> accounts)
    {
        var amountsId =  accounts
            .Select(account => account.Id)
            .ToList();

        var amounts = await GetAmountsAsync(amountsId);

        if (amounts == null)
            return null;

        foreach (var accountApplication in accounts)
            accountApplication.Amount =  amounts
                .FirstOrDefault(amount => amount.AccountId == accountApplication.Id)?
                .Amount;

        return accounts;
    }

    private async Task<List<AccountBalanceApplication>?> GetAmountsAsync(List<Guid> accountsId)
    {
        var tasks = accountsId
            .Chunk(5).Select(ids => _bankTransactionsClient
                .GetAccountsBalanceAsync(ids.ToList()));

        var results = await Task.WhenAll(tasks);

        if (results.Any(result => !result.Success))
            return null;
        
        return  results
            .SelectMany(result => result.GetContent())
            .ToList();  
    }
    
    public async Task<bool> MakeTransferAsync(Account account, decimal amount, string comments)
    {
        var transfer = new TransferApplication()
        {
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