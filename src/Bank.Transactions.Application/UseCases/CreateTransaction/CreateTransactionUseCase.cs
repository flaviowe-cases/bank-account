using Bank.Commons.Applications.Factories.Results;
using Bank.Transactions.Application.Gateways;
using Bank.Transactions.Application.Models;
using Bank.Transactions.Application.Repositories;
using FluentValidation;

namespace Bank.Transactions.Application.UseCases.CreateTransaction;

public class CreateTransactionUseCase(
    IValidator<CreateTransactionInput> validator,
    ICreateTransactionInputMapper mapper,
    IBankAccountClient bankAccountClient,
    ITransactionRepository transactionRepository,
    ITransactionProducer  transactionProducer,
    IResultFactory resultFactory) : ICreateTransactionUseCase
{
    private readonly IValidator<CreateTransactionInput> _validator = validator;
    private readonly ICreateTransactionInputMapper _mapper = mapper;
    private readonly IBankAccountClient _bankAccountClient = bankAccountClient;
    private readonly ITransactionRepository _transactionRepository = transactionRepository;
    private readonly ITransactionProducer _transactionProducer = transactionProducer;
    private readonly IResultFactory _resultFactory = resultFactory;

    public async Task<Result<CreateTransactionOutput>> HandleAsync(
        CreateTransactionInput input)
    {
        var validation = await _validator.ValidateAsync(input);

        if (!validation.IsValid)
            return _resultFactory.CreateFailure<CreateTransactionOutput>(
                "INVALID_FIELDS", "");

        var sourceAccountId = Guid.Empty;
        var destinationAccountId = Guid.Empty;

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
        
        var transaction = _mapper.MapToDomain(
            input, sourceAccountId, destinationAccountId);
        
        await _transactionRepository.AddAsync(transaction);
        
        await _transactionProducer.SendAsync(transaction);

        return _resultFactory.CreateSuccess(new CreateTransactionOutput()
        {
            TransactionId = transaction.Id
        });
    }
    
    private Result<CreateTransactionOutput> HandleAccountNotFound(
        string accountNotFoundErrorCode, Result<BankAccount> accountResult)
    {
        if (accountResult.ContainsFailure("ACCOUNT_NOT_FOUND"))
        {
            return _resultFactory.CreateFailure<CreateTransactionOutput>(
                accountNotFoundErrorCode,
                "Account not found");
        }
        else
        {
            return _resultFactory.CreateFailure<CreateTransactionOutput>(
                "SERVICE_TEMPORARILY_UNAVAILABLE",
                "Service temporarily unavailable");
        }
    }
}