using System.Collections.Concurrent;
using Bank.Transactions.Application.Models;
using Bank.Transactions.Application.Repositories;
using Bank.Transactions.Domain.Entities;
using Microsoft.Extensions.Logging;

namespace Bank.Transactions.Application.Services;

public class TransactionService(
    ILogger<TransactionService> logger,
    IAmountService amountService,
    TransferParameters transferParameters,
    ITransactionRepository transactionRepository,
    ConcurrentDictionary<Guid, SemaphoreSlim> concurrent) : ITransactionService
{
    private readonly ILogger<TransactionService> _logger = logger;
    private readonly IAmountService _amountService = amountService;
    private readonly TransferParameters _transferParameters = transferParameters;
    private readonly ITransactionRepository _transactionRepository = transactionRepository;
    private readonly ConcurrentDictionary<Guid, SemaphoreSlim> _concurrent = concurrent;

    public async Task<Transaction> ExecuteAsync(Transaction transaction)
    {
        var semaphoreId = transaction.SourceAccountId;

        if (semaphoreId.Equals(Guid.Empty))
            semaphoreId = Guid.NewGuid();

        var semaphoreSlim = _concurrent.GetOrAdd(
            semaphoreId, new SemaphoreSlim(
                initialCount: 1, maxCount: 1));

        await semaphoreSlim.WaitAsync();

        try
        {
            if (!await CheckTransactionLimit(transaction))
                return transaction;

            if (!await CheckSourceAccountFunds(transaction))
                return transaction;

            transaction.Status = TransactionStatusType.Success;
            await _transactionRepository.UpdateAsync(transaction);
            return transaction;
        }
        catch (Exception e)
        {
            _logger.LogCritical(e,
                "Error executing transfer " +
                "from {SourceAccount} " +
                "to {DestinationAccount}",
                transaction.SourceAccountId,
                transaction.DestinationAccountId);

            throw;
        }
        finally
        {
            semaphoreSlim.Release();
        }
    }

    private async Task<bool> CheckTransactionLimit(Transaction transaction)
    {
        if (transaction.Amount <= _transferParameters.LimitAmountTransfer)
            return true;

        if (transaction.SourceAccountId.Equals(Guid.Empty))
            return true;

        transaction.Status = TransactionStatusType.LimitExceeded;
        await _transactionRepository.UpdateAsync(transaction);
        return false;
    }

    private async Task<bool> CheckSourceAccountFunds(Transaction transaction)
    {
        if (transaction.SourceAccountId.Equals(Guid.Empty))
            return true;

        var currentBalance = await _amountService
            .GetCurrentBalanceAsync(transaction.SourceAccountId);

        if (currentBalance.Amount >= transaction.Amount)
            return true;

        transaction.Status = TransactionStatusType.InsufficientFunds;
        await _transactionRepository.UpdateAsync(transaction);
        return false;
    }
}