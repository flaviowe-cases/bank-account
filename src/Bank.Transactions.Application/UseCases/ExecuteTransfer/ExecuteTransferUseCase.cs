using Bank.Transactions.Application.Factories.Results;
using Bank.Transactions.Application.Gateways;
using Bank.Transactions.Application.Services;
using Bank.Transactions.Domain.Entities;
using FluentValidation;

namespace Bank.Transactions.Application.UseCases.ExecuteTransfer;

public class ExecuteTransferUseCase(
    IValidator<ExecuteTransferInput> validator,
    IBankAccountClient bankAccountClient,
    ITransferService  transferService,
    IResultFactory resultFactory)
{
    private readonly IValidator<ExecuteTransferInput> _validator = validator;
    private readonly IBankAccountClient _bankAccountClient = bankAccountClient;
    private readonly ITransferService _transferService = transferService;
    private readonly IResultFactory _resultFactory = resultFactory;

    public async Task<Result<ExecuteTransferOutput>> HandleAsync(ExecuteTransferInput  input)
    {
        var validation = await _validator.ValidateAsync(input);

        var sourceAccountId = Guid.Empty;
        var destinationAccountId = Guid.Empty;
        
        if (!validation.IsValid)
            return _resultFactory.CreateFailure<ExecuteTransferOutput>(
                "INVALID_FIELDS", 
                "Invalid fields specified",
                validation.Errors);

        if (input.SourceAccountNumber != 0)
        {
            var accountResult = await _bankAccountClient
                .GetByAccountNumber(input.SourceAccountNumber);

            if (!accountResult.Success)
                _resultFactory.CreateFailure<ExecuteTransferOutput>(
                    "SOURCE_ACCOUNT_NOT_FOUNT", 
                    "Source account not found");

            sourceAccountId = accountResult
                .GetContent().Id;
        }

        if (input.DestinationAccountNumber != 0)
        {
            var accountResult = await _bankAccountClient
                .GetByAccountNumber(input.DestinationAccountNumber);

            if (!accountResult.Success)
                _resultFactory.CreateFailure<ExecuteTransferOutput>(
                    "DESTINATION_ACCOUNT_NOT_FOUNT", 
                    "Destination account not found");
        
            destinationAccountId = accountResult.GetContent().Id;
        }

        var transaction = await _transferService.ExecuteAsync(
            sourceAccountId, destinationAccountId, input.Amount, input.Comments);

        return transaction.Status switch
        {
            TransactionStatusType.Success 
                => _resultFactory.CreateSuccess(new ExecuteTransferOutput()),
                
            TransactionStatusType.InsufficientFunds 
                => _resultFactory.CreateFailure<ExecuteTransferOutput>(
                    "INSUFFICIENT_FUNDS",
                    "Insufficient funds"),
            
            TransactionStatusType.LimitExceeded 
                => _resultFactory.CreateFailure<ExecuteTransferOutput>(
                    "LIMIT_EXCEEDED",
                    "Limit exceeded"),
            _ 
                => _resultFactory.CreateFailure<ExecuteTransferOutput>(
                    "TRANSFER_FAILED",
                    "Transfer failed"),
        };
    }
    
}