using Bank.Transactions.Application.Factories.Results;
using Bank.Transactions.Application.Gateways;
using Bank.Transactions.Application.Models;
using Bank.Transactions.Application.Services;
using Bank.Transactions.Domain.Entities;
using FluentValidation;

namespace Bank.Transactions.Application.UseCases.ExecuteTransaction;

public class ExecuteTransactionUseCase(
    IValidator<ExecuteTransactionInput> validator,
    IBankAccountClient bankAccountClient,
    ITransactionService transactionService,
    IResultFactory resultFactory) : IExecuteTransactionUseCase
{
    private readonly IValidator<ExecuteTransactionInput> _validator = validator;
    private readonly IBankAccountClient _bankAccountClient = bankAccountClient;
    private readonly ITransactionService _transactionService = transactionService;
    private readonly IResultFactory _resultFactory = resultFactory;

    public async Task<Result<ExecuteTransactionOutput>> HandleAsync(ExecuteTransactionInput input)
    {
        var validation = await _validator.ValidateAsync(input);

        var sourceAccountId = Guid.Empty;
        var destinationAccountId = Guid.Empty;

        if (!validation.IsValid)
            return _resultFactory.CreateFailure<ExecuteTransactionOutput>(
                "INVALID_FIELDS",
                "Invalid fields specified",
                validation.Errors);

        if (input.SourceAccountNumber.HasValue)
        {
            var accountNumber = input.SourceAccountNumber.Value;
            var accountResult = await _bankAccountClient
                .GetByAccountAsync(accountNumber);

            if (!accountResult.Success)
                return HandleAccountNotFound(
                    "SOURCE_ACCOUNT_NOT_FOUNT", accountResult);

            sourceAccountId = accountResult
                .GetContent().Id;
        }

        if (input.DestinationAccountNumber.HasValue)
        {
            var accountNumber = input.DestinationAccountNumber.Value;
            var accountResult = await _bankAccountClient
                .GetByAccountAsync(accountNumber);

            if (!accountResult.Success)
                return HandleAccountNotFound(
                    "DESTINATION_ACCOUNT_NOT_FOUNT", accountResult);

            destinationAccountId = accountResult.GetContent().Id;
        }

        var transaction = await _transactionService.ExecuteAsync(
            sourceAccountId, destinationAccountId, input.Amount, input.Comments);

        return transaction.Status switch
        {
            TransactionStatusType.Success
                => _resultFactory.CreateSuccess(new ExecuteTransactionOutput()),

            TransactionStatusType.InsufficientFunds
                => _resultFactory.CreateFailure<ExecuteTransactionOutput>(
                    "INSUFFICIENT_FUNDS",
                    "Insufficient funds"),

            TransactionStatusType.LimitExceeded
                => _resultFactory.CreateFailure<ExecuteTransactionOutput>(
                    "LIMIT_EXCEEDED",
                    "Limit exceeded"),
            _
                => _resultFactory.CreateFailure<ExecuteTransactionOutput>(
                    "TRANSFER_FAILED",
                    "Transfer failed"),
        };
    }

    private Result<ExecuteTransactionOutput> HandleAccountNotFound(
        string accountNotFoundErrorCode, Result<BankAccount> accountResult)
    {
        if (accountResult.ContainsFailure("ACCOUNT_NOT_FOUND"))
        {
            return _resultFactory.CreateFailure<ExecuteTransactionOutput>(
                accountNotFoundErrorCode,
                "Account not found");
        }
        else
        {
            return _resultFactory.CreateFailure<ExecuteTransactionOutput>(
                "SERVICE_TEMPORARILY_UNAVAILABLE",
                "Service temporarily unavailable");
        }
    }
}