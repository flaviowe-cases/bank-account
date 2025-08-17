using Bank.Transactions.Application.Factories.Results;
using Bank.Transactions.Application.Gateways;
using Bank.Transactions.Application.Models;
using Bank.Transactions.Application.Repositories;
using Bank.Transactions.Domain.Entities;
using FluentValidation;

namespace Bank.Transactions.Application.UseCases.GetTransactionsHistory;

public class GetTransactionsHistoryUseCase(
    IValidator<GetTransactionsHistoryInput> validator,
    IBankAccountClient bankAccountClient,
    ITransactionRepository repository,
    IResultFactory resultFactory) : IGetTransactionsHistoryUseCase
{
    private readonly IValidator<GetTransactionsHistoryInput> _validator = validator;
    private readonly IBankAccountClient _bankAccountClient = bankAccountClient;
    private readonly ITransactionRepository _repository = repository;
    private readonly IResultFactory _resultFactory = resultFactory;

    public async Task<Result<GetTransactionsHistoryOutput>> HandleAsync(
        GetTransactionsHistoryInput input)
    {
        var validation = await _validator.ValidateAsync(input);

        if (!validation.IsValid)
            return _resultFactory.CreateFailure<GetTransactionsHistoryOutput>(
                "INVALID_FIELDS", "Invalid fields provided");

        var accountResult = await GetAccountAsync(input.AccountNumber);

        if (!accountResult.Success)
            return _resultFactory
                .CreateFailure<GetTransactionsHistoryOutput>(
                    accountResult);

        var account = accountResult.GetContent();

        var history = await CreateHistoryAsync(account.Id);

        return _resultFactory.CreateSuccess(new GetTransactionsHistoryOutput()
        {
            History = history,
        });
    }

    private async Task<List<TransactionHistory>> CreateHistoryAsync(Guid accountId)
    {
        var transactions = await _repository
            .GetByAccountIdAsync(accountId);

        var accountsId = transactions
            .Where(transaction => !transaction.DestinationAccountId.Equals(Guid.Empty))
            .Select(transaction => transaction.DestinationAccountId)
            .ToList();

        accountsId.AddRange(transactions
            .Where(transaction => !transaction.SourceAccountId.Equals(Guid.Empty))
            .Select(transaction => transaction.SourceAccountId));

        var tasks = accountsId
            .Distinct()
            .Select(GetAccountAsync);

        var accounts = await Task.WhenAll(tasks);
        List<TransactionHistory> history = [];
        
        history.AddRange(transactions
            .Select(transaction => new TransactionHistory()
        {
            TimeStamp = transaction.TimeStamp,
            TransactionId = transaction.Id,
            Amount = transaction.Amount,
            OperationType = GetTransactionType(transaction),
            TransactionType = GetOperationType(transaction, accountId),
            AccountSource = accounts.FirstOrDefault(
                account => account.Id == transaction.SourceAccountId)?.Number,
            AccountDestination = accounts.FirstOrDefault(
                account => account.Id == transaction.DestinationAccountId)?.Number,
            Status = GetStatus(transaction.Status),
            Comments = transaction.Comments
        }));

        return history
            .OrderByDescending(historyItem => historyItem.TimeStamp)
            .ToList();
    }

    private static string GetStatus(TransactionStatusType transactionStatus)
    {
        return transactionStatus switch
        {
            TransactionStatusType.Success => "success",
            TransactionStatusType.InsufficientFunds => "insufficientFunds",
            TransactionStatusType.LimitExceeded => "limitExceeded",
            _ => "unknow"
        };
    }

    private static string GetTransactionType(Transaction transaction)
    {
        if (transaction.SourceAccountId.Equals(Guid.Empty))
            return "deposit";
        
        if (transaction.DestinationAccountId.Equals(Guid.Empty))
            return "withdrawal";
        
        return "transferBetweenAccounts";
    }

    private static string GetOperationType(Transaction transaction, Guid accountId)
    {
        if (transaction.SourceAccountId.Equals(accountId))
            return "debit";

        if (transaction.DestinationAccountId.Equals(accountId))
            return "credit";

        return "unknown";
    }

    private async Task<Result<BankAccount>> GetAccountAsync(int accountNumber)
    {
        var accountResult = await _bankAccountClient
            .GetByAccountAsync(accountNumber);

        if (accountResult.Success)
            return accountResult;

        if (accountResult.ContainsFailure("ACCOUNT_NOT_FOUND"))
            return _resultFactory.CreateFailure<BankAccount>(
                "ACCOUNT_NOT_FOUND", "Account not found");

        return _resultFactory.CreateFailure<BankAccount>(
            "SERVICE_TEMPORARILY_UNAVAILABLE",
            "Service temporarily unavailable");
    }

    private async Task<BankAccount> GetAccountAsync(Guid accountId)
    {
        var result = await _bankAccountClient.GetByAccountAsync(accountId);
        return result.Success ? 
            result.GetContent() : 
            new BankAccount { Id = accountId, Name = "", Number = 0 };
    }
}